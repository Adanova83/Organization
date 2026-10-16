using Library.Domain;
using Library.Repository;

namespace Library.Handler;

/// <summary>
/// Contrôle d'une inscription avant son enregistrement (création ou modification) : rassemble l'élément visé, ses inscrits,
/// sa capacité, son âge minimum et sa date de début, puis applique <see cref="SubscriptionRules"/>.
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

    /// <summary>Créneau d'atelier : âge minimum de l'activité, début de la session.</summary>
    public async Task EnsureAsync(Domain.ActivitySessionSlotSubscription subscription, bool canOverride, CancellationToken ct)
    {
        var slot = Required((await _slots.SearchAsync(subscription.SessionSlotId, ct)).Data, "session slot");
        var session = Required((await _sessions.SearchAsync(slot.SessionId, ct)).Data, "session");
        var activity = Required((await _activities.SearchAsync(session.ActivityId, ct)).Data, "activity");
        var others = (await _slotSubscriptions.SearchAsync(slot.Id, null, 0, 0, ct)).Data
            .Where(item => item.Id != subscription.Id).Select(item => item.MemberId).ToList();

        await CheckAsync(subscription.MemberId, others, slot.Capacity, activity.AgeMinimum, session.EffectiveStartDate, canOverride, ct);
    }

    /// <summary>Événement : son âge minimum et sa date.</summary>
    public async Task EnsureAsync(Domain.EventSubscription subscription, bool canOverride, CancellationToken ct)
    {
        var item = Required((await _events.SearchAsync(subscription.EventId, ct)).Data, "event");
        var others = (await _eventSubscriptions.SearchAsync(item.Id, null, 0, 0, ct)).Data
            .Where(other => other.Id != subscription.Id).Select(other => other.MemberId).ToList();

        await CheckAsync(subscription.MemberId, others, item.Capacity, item.AgeMinimum ?? 0, item.StartDate, canOverride, ct);
    }

    /// <summary>Créneau de projet : âge minimum du projet, début du projet (sinon de son année).</summary>
    public async Task EnsureAsync(Domain.ProjectSlotSubscription subscription, bool canOverride, CancellationToken ct)
    {
        var slot = Required((await _projectSlots.SearchAsync(subscription.ProjectSlotId, ct)).Data, "project slot");
        var project = Required((await _projects.SearchAsync(slot.ProjectId, ct)).Data, "project");
        var others = (await _projectSubscriptions.SearchAsync(slot.Id, null, 0, 0, ct)).Data
            .Where(item => item.Id != subscription.Id).Select(item => item.MemberId).ToList();

        await CheckAsync(subscription.MemberId, others, slot.Capacity, project.AgeMinimum, project.EffectiveStartDate, canOverride, ct);
    }

    private async Task CheckAsync(
        Guid memberId, IReadOnlyCollection<Guid> others, int? capacity, int ageMinimum, DateTime? startUtc, bool canOverride, CancellationToken ct)
    {
        DateOnly? birthdate = null;
        if (ageMinimum > 0 && !canOverride)
        {
            var member = Required((await _members.SearchAsync(memberId, ct)).Data, "member");
            birthdate = member.Birthdate is { } born ? DateOnly.FromDateTime(ParisTime.ToLocal(born)) : null;
        }

        var today = DateOnly.FromDateTime(ParisTime.ToLocal(DateTime.UtcNow));
        var start = startUtc is { } value ? DateOnly.FromDateTime(ParisTime.ToLocal(value)) : (DateOnly?)null;

        SubscriptionRules.Check(memberId, others, capacity, ageMinimum, birthdate, SubscriptionRules.AgeReference(start, today), canOverride);
    }

    private static T Required<T>(T? value, string what) where T : class
        => value ?? throw new ArgumentException($"The {what} does not exist.");
}
