using Library.Domain;
using Library.Repository;

namespace Library.Handler;

/// <summary>
/// Inscriptions aux créneaux d'ateliers, événements et créneaux de projets :
/// avant l'enregistrement, applique <see cref="SubscriptionRules"/> et décide « inscrit » ou « liste d'attente » ;
/// après une désinscription, fait passer les suivants de la liste d'attente ; donne les places occupées par élément.
/// </summary>
public sealed class SubscriptionGuard
{
    private readonly IMember _members;
    private readonly IActivitySessionSlot _slots;
    private readonly IActivitySession _sessions;
    private readonly IActivity _activities;
    private readonly IEvent _events;
    private readonly IProjectSlot _projectSlots;
    private readonly IProject _projects;
    private readonly IActivitySessionSlotSubscription _slotSubscriptions;
    private readonly IEventSubscription _eventSubscriptions;
    private readonly IProjectSlotSubscription _projectSubscriptions;

    public SubscriptionGuard(
        IMember members, IActivitySessionSlot slots, IActivitySession sessions, IActivity activities,
        IEvent events, IProjectSlot projectSlots, IProject projects,
        IActivitySessionSlotSubscription slotSubscriptions, IEventSubscription eventSubscriptions,
        IProjectSlotSubscription projectSubscriptions)
    {
        _members = members;
        _slots = slots;
        _sessions = sessions;
        _activities = activities;
        _events = events;
        _projectSlots = projectSlots;
        _projects = projects;
        _slotSubscriptions = slotSubscriptions;
        _eventSubscriptions = eventSubscriptions;
        _projectSubscriptions = projectSubscriptions;
    }

    // ---- Avant l'enregistrement : règles et liste d'attente ----

    /// <summary>Créneau d'atelier : âge minimum de l'activité, début de la session.</summary>
    public async Task EnsureAsync(Domain.ActivitySessionSlotSubscription subscription, bool canOverride, CancellationToken ct)
    {
        var slot = Required((await _slots.SearchAsync(subscription.SessionSlotId, ct)).Data, "session slot");
        var session = Required((await _sessions.SearchAsync(slot.SessionId, ct)).Data, "session");
        var activity = Required((await _activities.SearchAsync(session.ActivityId, ct)).Data, "activity");
        var current = (await _slotSubscriptions.SearchAsync(slot.Id, null, 0, 0, ct)).Data
            .Select(item => (item.Id, item.MemberId, item.IsWaiting)).ToList();

        subscription.IsWaiting = await DecideAsync(subscription.Id, subscription.MemberId, current,
            slot.Capacity, activity.AgeMinimum, session.EffectiveStartDate, canOverride, ct);
    }

    /// <summary>Événement : son âge minimum et sa date.</summary>
    public async Task EnsureAsync(Domain.EventSubscription subscription, bool canOverride, CancellationToken ct)
    {
        var item = Required((await _events.SearchAsync(subscription.EventId, ct)).Data, "event");
        var current = (await _eventSubscriptions.SearchAsync(item.Id, null, 0, 0, ct)).Data
            .Select(other => (other.Id, other.MemberId, other.IsWaiting)).ToList();

        subscription.IsWaiting = await DecideAsync(subscription.Id, subscription.MemberId, current,
            item.Capacity, item.AgeMinimum ?? 0, item.StartDate, canOverride, ct);
    }

    /// <summary>Créneau de projet : âge minimum du projet, début du projet (sinon de son année).</summary>
    public async Task EnsureAsync(Domain.ProjectSlotSubscription subscription, bool canOverride, CancellationToken ct)
    {
        var slot = Required((await _projectSlots.SearchAsync(subscription.ProjectSlotId, ct)).Data, "project slot");
        var project = Required((await _projects.SearchAsync(slot.ProjectId, ct)).Data, "project");
        var current = (await _projectSubscriptions.SearchAsync(slot.Id, null, 0, 0, ct)).Data
            .Select(item => (item.Id, item.MemberId, item.IsWaiting)).ToList();

        subscription.IsWaiting = await DecideAsync(subscription.Id, subscription.MemberId, current,
            slot.Capacity, project.AgeMinimum, project.EffectiveStartDate, canOverride, ct);
    }

    /// <returns>Vrai : liste d'attente.</returns>
    private async Task<bool> DecideAsync(
        Guid subscriptionId, Guid memberId, List<(Guid Id, Guid MemberId, bool IsWaiting)> current,
        int? capacity, int ageMinimum, DateTime? startUtc, bool canOverride, CancellationToken ct)
    {
        // Modification sans changement de membre ni d'élément : l'état (inscrit ou en attente) est conservé.
        var existing = current.FindIndex(item => item.Id == subscriptionId);
        if (subscriptionId != Guid.Empty && existing >= 0 && current[existing].MemberId == memberId)
        {
            return current[existing].IsWaiting;
        }

        var others = current.Where(item => item.Id != subscriptionId).ToList();

        DateOnly? birthdate = null;
        if (ageMinimum > 0 && !canOverride)
        {
            var member = Required((await _members.SearchAsync(memberId, ct)).Data, "member");
            birthdate = member.Birthdate is { } born ? DateOnly.FromDateTime(ParisTime.ToLocal(born)) : null;
        }

        var today = DateOnly.FromDateTime(ParisTime.ToLocal(DateTime.UtcNow));
        var start = startUtc is { } value ? DateOnly.FromDateTime(ParisTime.ToLocal(value)) : (DateOnly?)null;

        return SubscriptionRules.Check(
            memberId,
            others.Where(item => !item.IsWaiting).Select(item => item.MemberId).ToList(),
            others.Where(item => item.IsWaiting).Select(item => item.MemberId).ToList(),
            capacity, ageMinimum, birthdate, SubscriptionRules.AgeReference(start, today), canOverride);
    }

    // ---- Après une désinscription : les suivants de la liste d'attente prennent les places libérées ----

    public async Task AfterDeleteAsync(Domain.ActivitySessionSlotSubscription subscription, CancellationToken ct)
    {
        var slot = (await _slots.SearchAsync(subscription.SessionSlotId, ct)).Data;
        if (slot is not null)
        {
            await FillAsync(slot.Id, slot.Capacity, _slotSubscriptions.CountByTargetAsync,
                async (target, token) => await _slotSubscriptions.PromoteNextAsync(target, token) is not null, ct);
        }
    }

    public async Task AfterDeleteAsync(Domain.EventSubscription subscription, CancellationToken ct)
    {
        var item = (await _events.SearchAsync(subscription.EventId, ct)).Data;
        if (item is not null)
        {
            await FillAsync(item.Id, item.Capacity, _eventSubscriptions.CountByTargetAsync,
                async (target, token) => await _eventSubscriptions.PromoteNextAsync(target, token) is not null, ct);
        }
    }

    public async Task AfterDeleteAsync(Domain.ProjectSlotSubscription subscription, CancellationToken ct)
    {
        var slot = (await _projectSlots.SearchAsync(subscription.ProjectSlotId, ct)).Data;
        if (slot is not null)
        {
            await FillAsync(slot.Id, slot.Capacity, _projectSubscriptions.CountByTargetAsync,
                async (target, token) => await _projectSubscriptions.PromoteNextAsync(target, token) is not null, ct);
        }
    }

    /// <summary>Fait passer les premiers de la liste d'attente tant qu'il reste des places (toutes si la capacité est illimitée).</summary>
    private static async Task FillAsync(
        Guid target, int? capacity,
        Func<IReadOnlyCollection<Guid>, CancellationToken, Task<IReadOnlyDictionary<Guid, (int Confirmed, int Waiting)>>> count,
        Func<Guid, CancellationToken, Task<bool>> promoteNext, CancellationToken ct)
    {
        while (true)
        {
            var (confirmed, waiting) = (await count([target], ct)).GetValueOrDefault(target);
            if (waiting == 0 || (capacity is { } places && confirmed >= places) || !await promoteNext(target, ct))
            {
                return;
            }
        }
    }

    // ---- Places occupées (sans les noms) ----

    public async Task<IReadOnlyList<Contract.SubscriptionAvailability>> AvailabilityAsync(
        Contract.SubscriptionTargetKind kind, IReadOnlyCollection<Guid> targetIds, CancellationToken ct)
    {
        var counts = kind switch
        {
            Contract.SubscriptionTargetKind.ActivitySlot => await _slotSubscriptions.CountByTargetAsync(targetIds, ct),
            Contract.SubscriptionTargetKind.Event => await _eventSubscriptions.CountByTargetAsync(targetIds, ct),
            _ => await _projectSubscriptions.CountByTargetAsync(targetIds, ct)
        };

        var result = new List<Contract.SubscriptionAvailability>();
        foreach (var id in targetIds.Distinct())
        {
            int? capacity = kind switch
            {
                Contract.SubscriptionTargetKind.ActivitySlot => (await _slots.SearchAsync(id, ct)).Data?.Capacity,
                Contract.SubscriptionTargetKind.Event => (await _events.SearchAsync(id, ct)).Data?.Capacity,
                _ => (await _projectSlots.SearchAsync(id, ct)).Data?.Capacity
            };

            var (confirmed, waiting) = counts.GetValueOrDefault(id);
            result.Add(new Contract.SubscriptionAvailability
            {
                TargetId = id,
                Capacity = capacity,
                Subscribed = confirmed,
                Waiting = waiting
            });
        }

        return result;
    }

    private static T Required<T>(T? value, string what) where T : class
        => value ?? throw new ArgumentException($"The {what} does not exist.");
}
