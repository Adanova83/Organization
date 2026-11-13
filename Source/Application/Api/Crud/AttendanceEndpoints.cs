using Api.Calendar;
using Api.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ct = Library.Contract;
using H = Library.Handler;

namespace Api.Crud;

/// <summary>
/// Présences (feuilles d'émargement) des ateliers ; pas d'appel pour les événements.
/// Lecture : animateurs du créneau, ou subscription.read. Appel : animateurs du créneau (méthode « appel »), ou
/// subscription.edit (méthode « correction d'un responsable »). Statistiques : subscription.read.
///   GET /api/attendances/slots/{slotId}          séances et décomptes
///   GET /api/attendances/slots/{slotId}/{date}   feuille d'émargement (date : yyyy-MM-dd)
///   PUT /api/attendances/slots/{slotId}/{date}   appel : [{ memberId, status }]
///   GET /api/attendances/stats?from=&amp;to=        fréquentation sur une période
/// </summary>
public static class AttendanceEndpoints
{
    public static IEndpointRouteBuilder MapAttendances(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attendances").WithTags("Attendances").RequireAccess();

        group.MapGet("/stats", async (DateOnly from, DateOnly to, [FromServices] H.Attendance attendances, CancellationToken ct)
                => Results.Ok(await attendances.StatsAsync(from, to, ct)))
            .RequirePermission(Permissions.SubscriptionRead);

        group.MapGet("/slots/{slotId:guid}", async (
            Guid slotId, [FromServices] H.Attendance attendances, HttpContext http, CancellationToken ct) =>
        {
            if (await AccessAsync(http, slotId, write: false, ct) is null)
            {
                return Forbidden();
            }

            var occurrences = await attendances.OccurrencesAsync(slotId, MondayFirst(http), ct);
            return occurrences is null ? Results.NotFound() : Results.Ok(occurrences);
        });

        group.MapGet("/slots/{slotId:guid}/{date}", async (
            Guid slotId, DateOnly date, [FromServices] H.Attendance attendances, HttpContext http, CancellationToken ct) =>
        {
            if (await AccessAsync(http, slotId, write: false, ct) is null)
            {
                return Forbidden();
            }

            var sheet = await attendances.SheetAsync(slotId, date, MondayFirst(http), ct);
            return sheet is null ? Results.NotFound() : Results.Ok(sheet);
        });

        group.MapPut("/slots/{slotId:guid}/{date}", async (
            Guid slotId, DateOnly date, List<Ct.AttendanceEntry> entries,
            [FromServices] H.Attendance attendances, HttpContext http, CancellationToken ct) =>
        {
            if (await AccessAsync(http, slotId, write: true, ct) is not { } method)
            {
                return Forbidden();
            }

            if (!await attendances.RecordAsync(slotId, date, entries, method, MondayFirst(http), ct))
            {
                return Results.NotFound();
            }

            return Results.Ok(await attendances.SheetAsync(slotId, date, MondayFirst(http), ct));
        });

        return app;
    }

    private static bool MondayFirst(HttpContext http)
        => http.RequestServices.GetRequiredService<IOptions<CalendarFeedOptions>>().Value.DayOfWeekStartsOnMonday;

    /// <summary>
    /// Méthode d'enregistrement autorisée, ou null : les animateurs du créneau font l'appel ; les responsables
    /// (subscription.read pour lire, subscription.edit pour écrire) corrigent.
    /// </summary>
    private static async Task<Ct.AttendanceMethod?> AccessAsync(HttpContext http, Guid slotId, bool write, CancellationToken ct)
    {
        var (me, _) = await http.RequestServices.GetRequiredService<CurrentMember>().FindAsync(http.User, ct);
        if (me is not null && await http.RequestServices.GetRequiredService<H.Participant>()
                .IsAnimatorAsync(Ct.SubscriptionTargetKind.ActivitySlot, slotId, me.Id, ct))
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
        detail: "Only the animators of this slot, or staff with the subscription permissions, can see or record attendance.");
}
