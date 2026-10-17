using Api.Security;
using Microsoft.AspNetCore.Mvc;
using Ct = Library.Contract;
using H = Library.Handler;

namespace Api.Crud;

/// <summary>
/// Places occupées des créneaux et événements, sans les noms des inscrits : lisibles par tout membre qui voit l'élément
/// (activity.read, event.read ou project.read), pour afficher « 8 / 12 places » ou « Complet ».
/// GET /api/{route}/availability?ids=…&amp;ids=…
/// </summary>
public static class SubscriptionAvailabilityEndpoints
{
    public static IEndpointRouteBuilder MapSubscriptionAvailability(this IEndpointRouteBuilder app)
    {
        Map(app, "/api/activity-session-slot-subscriptions/availability", Ct.SubscriptionTargetKind.ActivitySlot, Permissions.ActivityRead);
        Map(app, "/api/event-subscriptions/availability", Ct.SubscriptionTargetKind.Event, Permissions.EventRead);
        Map(app, "/api/project-slot-subscriptions/availability", Ct.SubscriptionTargetKind.ProjectSlot, Permissions.ProjectRead);
        return app;
    }

    private static void Map(IEndpointRouteBuilder app, string route, Ct.SubscriptionTargetKind kind, string permission)
    {
        app.MapGet(route, async ([FromQuery] Guid[] ids, [FromServices] H.SubscriptionGuard guard, CancellationToken ct) =>
            {
                if (ids.Length > 500)
                {
                    throw new ArgumentException("At most 500 ids per request.", nameof(ids));
                }

                return Results.Ok(await guard.AvailabilityAsync(kind, ids, ct));
            })
            .WithTags("Subscriptions")
            .RequirePermission(permission);
    }
}
