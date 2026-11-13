using Api.Security;
using Microsoft.AspNetCore.Mvc;
using Ct = Library.Contract;
using H = Library.Handler;

namespace Api.Crud;

/// <summary>
/// Participants d'un créneau d'atelier, d'un événement ou d'un créneau de projet, avec leurs coordonnées et les
/// contacts de leur famille. Réservé à ceux qui encadrent : les animateurs de cet élément-là seulement, et les
/// responsables (subscription.read et member.read).
/// GET /api/participants/{activity-slot|event|project-slot}/{targetId}
/// </summary>
public static class ParticipantEndpoints
{
    public static IEndpointRouteBuilder MapParticipants(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/participants/{kind}/{targetId:guid}", async (
                string kind, Guid targetId,
                [FromServices] H.Participant participants, HttpContext http, CancellationToken ct) =>
            {
                Ct.SubscriptionTargetKind? target = kind switch
                {
                    "activity-slot" => Ct.SubscriptionTargetKind.ActivitySlot,
                    "event" => Ct.SubscriptionTargetKind.Event,
                    "project-slot" => Ct.SubscriptionTargetKind.ProjectSlot,
                    _ => null
                };

                if (target is null)
                {
                    return Results.NotFound();
                }

                if (!await CanSeeAsync(http, target.Value, targetId, participants, ct))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status403Forbidden,
                        title: "Forbidden.",
                        detail: "Only the animators of this item, or staff with subscription.read and member.read, can see its participants.");
                }

                return Results.Ok(await participants.ListAsync(target.Value, targetId, ct));
            })
            .WithTags("Participants")
            .RequireAccess();

        return app;
    }

    private static async Task<bool> CanSeeAsync(
        HttpContext http, Ct.SubscriptionTargetKind kind, Guid targetId, H.Participant participants, CancellationToken ct)
    {
        if (http.HasValidApiKey())
        {
            return true;
        }

        // Responsables : inscriptions et fiches de tous les membres.
        var permissions = http.RequestServices.GetRequiredService<PermissionService>();
        if (permissions.HasPermission(http.User, Permissions.SubscriptionRead)
            && permissions.HasPermission(http.User, Permissions.MemberRead))
        {
            return true;
        }

        // Animateurs : seulement ce qu'ils animent.
        var (me, _) = await http.RequestServices.GetRequiredService<CurrentMember>().FindAsync(http.User, ct);
        return me is not null && await participants.IsAnimatorAsync(kind, targetId, me.Id, ct);
    }
}
