using Api.Calendar;
using Api.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ct = Library.Contract;
using H = Library.Handler;

namespace Api.Crud;

/// <summary>
/// Présences (feuilles d'émargement) des ateliers et des événements.
/// Lecture : animateurs de la séance, ou subscription.read. Appel : animateurs de la séance (méthode « appel »), ou
/// subscription.edit (méthode « correction d'un responsable »). Statistiques : subscription.read.
///   GET /api/attendances/{activity-slot|event}/{targetId}          séances et décomptes
///   GET /api/attendances/{activity-slot|event}/{targetId}/{date}   feuille d'émargement (date : yyyy-MM-dd)
///   PUT /api/attendances/{activity-slot|event}/{targetId}/{date}   appel : [{ memberId, status }]
///   GET /api/attendances/stats?from=&amp;to=                           fréquentation sur une période
/// </summary>
public static class AttendanceEndpoints
{
    public static IEndpointRouteBuilder MapAttendances(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attendances").WithTags("Attendances").RequireAccess();

        group.MapGet("/stats", async (DateOnly from, DateOnly to, [FromServices] H.Attendance attendances, CancellationToken ct)
                => Results.Ok(await attendances.StatsAsync(from, to, ct)))
            .RequirePermission(Permissions.SubscriptionRead);

        group.MapGet("/{kind}/{targetId:guid}", async (
            string kind, Guid targetId, [FromServices] H.Attendance attendances, HttpContext http, CancellationToken ct) =>
        {
            if (ParseKind(kind) is not { } target)
            {
                return Results.NotFound();
            }

            if (await AccessAsync(http, target, targetId, write: false, ct) is null)
            {
                return Forbidden();
            }

            var occurrences = await attendances.OccurrencesAsync(target, targetId, MondayFirst(http), ct);
            return occurrences is null ? Results.NotFound() : Results.Ok(occurrences);
        });

        group.MapGet("/{kind}/{targetId:guid}/{date}", async (
            string kind, Guid targetId, DateOnly date, [FromServices] H.Attendance attendances, HttpContext http, CancellationToken ct) =>
        {
            if (ParseKind(kind) is not { } target)
            {
                return Results.NotFound();
            }

            if (await AccessAsync(http, target, targetId, write: false, ct) is null)
            {
                return Forbidden();
            }

            var sheet = await attendances.SheetAsync(target, targetId, date, MondayFirst(http), ct);
            return sheet is null ? Results.NotFound() : Results.Ok(sheet);
        });

        group.MapPut("/{kind}/{targetId:guid}/{date}", async (
            string kind, Guid targetId, DateOnly date, List<Ct.AttendanceEntry> entries,
            [FromServices] H.Attendance attendances, HttpContext http, CancellationToken ct) =>
        {
            if (ParseKind(kind) is not { } target)
            {
                return Results.NotFound();
            }

            if (await AccessAsync(http, target, targetId, write: true, ct) is not { } method)
            {
                return Forbidden();
            }

            if (!await attendances.RecordAsync(target, targetId, date, entries, method, MondayFirst(http), ct))
            {
                return Results.NotFound();
            }

            var sheet = await attendances.SheetAsync(target, targetId, date, MondayFirst(http), ct);
            return Results.Ok(sheet);
        });

        return app;
    }

    private static Ct.SubscriptionTargetKind? ParseKind(string kind) => kind switch
    {
        "activity-slot" => Ct.SubscriptionTargetKind.ActivitySlot,
        "event" => Ct.SubscriptionTargetKind.Event,
        _ => null
    };

    private static bool MondayFirst(HttpContext http)
        => http.RequestServices.GetRequiredService<IOptions<CalendarFeedOptions>>().Value.DayOfWeekStartsOnMonday;

    /// <summary>
    /// Méthode d'enregistrement autorisée, ou null : les animateurs de la séance font l'appel ; les responsables
    /// (subscription.read pour lire, subscription.edit pour écrire) corrigent.
    /// </summary>
    private static async Task<Ct.AttendanceMethod?> AccessAsync(
        HttpContext http, Ct.SubscriptionTargetKind kind, Guid targetId, bool write, CancellationToken ct)
    {
        var (me, _) = await http.RequestServices.GetRequiredService<CurrentMember>().FindAsync(http.User, ct);
        if (me is not null && await http.RequestServices.GetRequiredService<H.Participant>().IsAnimatorAsync(kind, targetId, me.Id, ct))
        {
            return Ct.AttendanceMethod.RollCall;
        }

        if (http.HasValidApiKey())
        {
            return Ct.AttendanceMethod.StaffCorrection;
        }

        var permissions = http.RequestServices.GetRequiredService<PermissionService>();
        return permissions.HasPermission(http.User, write ? Permissions.SubscriptionEdit : Permissions.SubscriptionRead)
            ? Ct.AttendanceMethod.StaffCorrection
            : null;
    }

    private static IResult Forbidden() => Results.Problem(
        statusCode: StatusCodes.Status403Forbidden,
        title: "Forbidden.",
        detail: "Only the animators of this session, or staff with the subscription permissions, can see or record attendance.");
}
