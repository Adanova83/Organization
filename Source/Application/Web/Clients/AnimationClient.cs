using Web.Security;
using Ct = Library.Contract;

namespace Web.Clients;

/// <summary>Animateur d'un élément (créneau d'activité ou créneau de projet ; pas d'animateurs pour les événements).</summary>
public sealed record AnimationItem(Guid Id, Guid TargetId, Guid MemberId, int Order);

/// <summary>
/// Regroupe les trois routes d'animateurs de l'API. Les éléments animés sont les mêmes que ceux auxquels on s'inscrit :
/// on réutilise donc <see cref="SubscriptionKind"/> pour les désigner.
/// </summary>
public sealed class AnimationClient
{
    /// <summary>Vrai si la sorte d'élément a des animateurs (les événements n'en ont pas).</summary>
    public static bool IsAnimated(SubscriptionKind kind) => kind != SubscriptionKind.Event;

    private readonly ApiClient _api;

    public AnimationClient(ApiClient api)
    {
        _api = api;
    }

    public static string Route(SubscriptionKind kind) => kind switch
    {
        SubscriptionKind.Slot => ApiRoutes.ActivitySlotAnimations,
        _ => ApiRoutes.ProjectSlotAnimations
    };

    /// <summary>Permission de lecture : celle du domaine de l'élément animé.</summary>
    public static string ReadPermission(SubscriptionKind kind) => kind switch
    {
        SubscriptionKind.Slot => Permissions.ActivityRead,
        _ => Permissions.ProjectRead
    };

    /// <summary>Permission d'écriture : celle du domaine de l'élément animé.</summary>
    public static string EditPermission(SubscriptionKind kind) => kind switch
    {
        SubscriptionKind.Slot => Permissions.ActivityEdit,
        _ => Permissions.ProjectEdit
    };

    public async Task<List<AnimationItem>> ListAsync(
        SubscriptionKind kind, Guid? targetId = null, Guid? memberId = null, CancellationToken ct = default)
    {
        var route = Route(kind);

        var items = kind switch
        {
            SubscriptionKind.Slot => (await _api.ListAllAsync<Ct.ActivitySessionSlotAnimation>(route, targetId, memberId, ct))
                .Select(item => new AnimationItem(item.Id, item.SessionSlotId, item.MemberId, item.Order)),
            _ => (await _api.ListAllAsync<Ct.ProjectSlotAnimation>(route, targetId, memberId, ct))
                .Select(item => new AnimationItem(item.Id, item.ProjectSlotId, item.MemberId, item.Order))
        };

        return items.OrderBy(item => item.Order).ToList();
    }

    public async Task CreateAsync(SubscriptionKind kind, Guid targetId, Guid memberId, int order, CancellationToken ct = default)
    {
        var route = Route(kind);

        switch (kind)
        {
            case SubscriptionKind.Slot:
                await _api.CreateAsync(route, new Ct.ActivitySessionSlotAnimation { SessionSlotId = targetId, MemberId = memberId, Order = order }, ct);
                break;
            default:
                await _api.CreateAsync(route, new Ct.ProjectSlotAnimation { ProjectSlotId = targetId, MemberId = memberId, Order = order }, ct);
                break;
        }
    }

    public Task DeleteAsync(SubscriptionKind kind, Guid animationId, CancellationToken ct = default)
        => _api.DeleteAsync(Route(kind), animationId, ct);
}
