using Ct = Library.Contract;

namespace Web.Clients;

/// <summary>Les trois sortes d'inscription : à un créneau d'activité, à un événement, à un créneau de projet.</summary>
public enum SubscriptionKind
{
    Slot,
    Event,
    Project
}

/// <summary>
/// Inscription, quelle que soit sa sorte : identifiant, élément auquel on s'inscrit (créneau ou événement), membre,
/// et, s'il est complet, place dans la liste d'attente (rang 1 = le prochain à passer).
/// </summary>
public sealed record SubscriptionItem(Guid Id, Guid TargetId, Guid MemberId, bool IsWaiting = false, int? WaitingRank = null);

/// <summary>Regroupe les trois routes d'inscription de l'API derrière une même interface.</summary>
public sealed class SubscriptionClient
{
    private readonly ApiClient _api;

    public SubscriptionClient(ApiClient api)
    {
        _api = api;
    }

    public static string Route(SubscriptionKind kind) => kind switch
    {
        SubscriptionKind.Slot => ApiRoutes.ActivitySlotSubscriptions,
        SubscriptionKind.Event => ApiRoutes.EventSubscriptions,
        _ => ApiRoutes.ProjectSlotSubscriptions
    };

    /// <summary>Segment d'adresse (/subscriptions/{segment}/{id}) d'une sorte d'inscription.</summary>
    public static string Segment(SubscriptionKind kind) => kind.ToString().ToLowerInvariant();

    public static SubscriptionKind? Parse(string? segment) => segment?.ToLowerInvariant() switch
    {
        "slot" => SubscriptionKind.Slot,
        "event" => SubscriptionKind.Event,
        "project" => SubscriptionKind.Project,
        _ => null
    };

    public async Task<List<SubscriptionItem>> ListAsync(
        SubscriptionKind kind, Guid? targetId = null, Guid? memberId = null, CancellationToken ct = default)
    {
        var route = Route(kind);

        switch (kind)
        {
            case SubscriptionKind.Slot:
                return (await _api.ListAllAsync<Ct.ActivitySessionSlotSubscription>(route, targetId, memberId, ct))
                    .Select(item => new SubscriptionItem(item.Id, item.SessionSlotId, item.MemberId, item.IsWaiting, item.WaitingRank)).ToList();
            case SubscriptionKind.Event:
                return (await _api.ListAllAsync<Ct.EventSubscription>(route, targetId, memberId, ct))
                    .Select(item => new SubscriptionItem(item.Id, item.EventId, item.MemberId, item.IsWaiting, item.WaitingRank)).ToList();
            default:
                return (await _api.ListAllAsync<Ct.ProjectSlotSubscription>(route, targetId, memberId, ct))
                    .Select(item => new SubscriptionItem(item.Id, item.ProjectSlotId, item.MemberId, item.IsWaiting, item.WaitingRank)).ToList();
        }
    }

    public async Task CreateAsync(SubscriptionKind kind, Guid targetId, Guid memberId, CancellationToken ct = default)
    {
        var route = Route(kind);

        switch (kind)
        {
            case SubscriptionKind.Slot:
                await _api.CreateAsync(route, new Ct.ActivitySessionSlotSubscription { SessionSlotId = targetId, MemberId = memberId }, ct);
                break;
            case SubscriptionKind.Event:
                await _api.CreateAsync(route, new Ct.EventSubscription { EventId = targetId, MemberId = memberId }, ct);
                break;
            default:
                await _api.CreateAsync(route, new Ct.ProjectSlotSubscription { ProjectSlotId = targetId, MemberId = memberId }, ct);
                break;
        }
    }

    /// <summary>Libellé et adresse de la fiche d'un élément auquel on est inscrit. Repli neutre si l'utilisateur ne peut pas le lire.</summary>
    public async Task<(string Title, string? Url)> DescribeAsync(SubscriptionKind kind, Guid targetId, CancellationToken ct = default)
    {
        try
        {
            switch (kind)
            {
                case SubscriptionKind.Slot:
                {
                    var slot = await _api.GetByIdAsync<Ct.ActivitySessionSlot>(ApiRoutes.ActivitySessionSlots, targetId, ct);
                    var session = await _api.GetByIdAsync<Ct.ActivitySession>(ApiRoutes.ActivitySessions, slot.SessionId, ct);
                    var activity = await _api.GetByIdAsync<Ct.Activity>(ApiRoutes.Activities, session.ActivityId, ct);
                    return ($"{activity.Name} – {Models.Format.Day(slot.DayOfWeek)} {Models.Format.Time(slot.StartTime)}–{Models.Format.Time(slot.EndTime)}",
                        $"/activities/{activity.Id}");
                }
                case SubscriptionKind.Event:
                {
                    var item = await _api.GetByIdAsync<Ct.Event>(ApiRoutes.Events, targetId, ct);
                    return ($"{item.Name} – {Models.Format.DateTimeLabel(item.StartDate)}", $"/events/{item.Id}");
                }
                default:
                {
                    var slot = await _api.GetByIdAsync<Ct.ProjectSlot>(ApiRoutes.ProjectSlots, targetId, ct);
                    var project = await _api.GetByIdAsync<Ct.Project>(ApiRoutes.Projects, slot.ProjectId, ct);
                    return ($"{project.Name} – {Models.Format.Day(slot.DayOfWeek)} {Models.Format.Time(slot.StartTime)}–{Models.Format.Time(slot.EndTime)}",
                        $"/projects/{project.Id}");
                }
            }
        }
        catch (Exception ex) when (ex is ApiException or HttpRequestException)
        {
            return ("Détails indisponibles", null);
        }
    }

    /// <summary>Places occupées (inscrits, liste d'attente) par élément, sans les noms ; vide si rien à demander.</summary>
    public async Task<Dictionary<Guid, Ct.SubscriptionAvailability>> AvailabilityAsync(
        SubscriptionKind kind, IEnumerable<Guid> targetIds, CancellationToken ct = default)
    {
        var ids = targetIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return [];
        }

        var query = string.Join("&", ids.Select(id => $"ids={id}"));
        var items = await _api.GetListAsync<Ct.SubscriptionAvailability>($"{Route(kind)}/availability?{query}", ct);
        return items.ToDictionary(item => item.TargetId);
    }

    public Task DeleteAsync(SubscriptionKind kind, Guid subscriptionId, CancellationToken ct = default)
        => _api.DeleteAsync(Route(kind), subscriptionId, ct);
}
