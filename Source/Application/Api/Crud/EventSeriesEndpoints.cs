using Api.Calendar;
using Api.Security;
using Microsoft.AspNetCore.Mvc;
using Ct = Library.Contract;
using Do = Library.Domain;
using H = Library.Handler;
using M = Library.Mapper;

namespace Api.Crud;

/// <summary>
/// Séries d'événements récurrents. Chaque occurrence est un événement à part entière (inscrits, modification, suppression
/// propres) ; les occurrences d'une série partagent un SeriesId. Lecture : event.read ; écriture : event.edit.
/// </summary>
public static class EventSeriesEndpoints
{
    public static RouteGroupBuilder MapEventSeries(this RouteGroupBuilder events)
    {
        events.MapPost("/series", CreateAsync).RequirePermission(Permissions.EventEdit);
        events.MapGet("/series/{seriesId:guid}", ListAsync);
        events.MapPut("/series/{seriesId:guid}", UpdateAsync).RequirePermission(Permissions.EventEdit);
        events.MapDelete("/series/{seriesId:guid}", DeleteAsync).RequirePermission(Permissions.EventEdit);
        return events;
    }

    /// <summary>
    /// POST /api/events/series : une occurrence à chaque date de la récurrence (jour de la semaine, numéro ou rang du jour
    /// dans le mois) sur la période de l'année scolaire de Event, à partir d'aujourd'hui si l'année a commencé
    /// (From / Until permettent de la restreindre). Heures de Paris, changements d'heure compris.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        Ct.EventSeries body, [FromServices] H.Event handler, [FromServices] H.SchoolYear schoolYears,
        [FromServices] H.Holiday holidays, CancellationToken ct)
    {
        if (body.Event is null)
        {
            throw new ArgumentException("Event is required.", nameof(body.Event));
        }

        if (body.DayOfWeek is < 0 or > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(body.DayOfWeek), "Day of week must be between 0 (Sunday) and 6 (Saturday).");
        }

        if (body.Event.SchoolYearId == Guid.Empty)
        {
            throw new ArgumentException("School year ID is required.", nameof(body.Event.SchoolYearId));
        }

        var year = (await schoolYears.SearchAsync(body.Event.SchoolYearId, ct)).Data
            ?? throw new ArgumentException("The school year does not exist.", nameof(body.Event.SchoolYearId));

        var today = DateOnly.FromDateTime(ParisTime.ToLocal(DateTime.UtcNow));
        var yearStart = DateOnly.FromDateTime(ParisTime.ToLocal(year.StartDate));
        var from = body.From ?? (yearStart > today ? yearStart : today);
        var until = body.Until ?? DateOnly.FromDateTime(ParisTime.ToLocal(year.EndDate));

        // Fin avant le début : l'événement se termine le lendemain (soirée qui passe minuit).
        var duration = body.EndTime - body.StartTime;

        if (!body.DuringTermTime && !body.DuringSchoolHolidays)
        {
            throw new ArgumentException("The series must take place during term time, during school holidays, or both.", nameof(body.DuringTermTime));
        }

        // Vacances scolaires (rattachées à une année) et jours fériés : appliqués selon le rythme choisi.
        var closedPeriods = (await holidays.SearchAsync(null, 0, 0, ct)).Data
            .Select(h => new Do.ClosedPeriod(
                DateOnly.FromDateTime(ParisTime.ToLocal(h.StartDate)),
                DateOnly.FromDateTime(ParisTime.ToLocal(h.EndDate)),
                h.SchoolYearId.HasValue))
            .ToList();

        var dates = Do.EventRecurrence.Dates(
            from, until, (Do.RecurrenceFrequency)body.Frequency, body.Interval,
            body.DayOfWeek is { } day ? (DayOfWeek)day : null, body.DayOfMonth, body.WeekOfMonth)
            .Where(date => Do.SessionSchedule.IsActiveOn(date, body.DuringTermTime, body.DuringSchoolHolidays, closedPeriods))
            .ToList();

        if (dates.Count == 0)
        {
            throw new ArgumentException("The recurrence has no date in the period of the school year.", nameof(body.Frequency));
        }

        var occurrences = dates.Select(date =>
        {
            var occurrence = M.Event.ContractToDomain(body.Event, Guid.Empty);
            var start = date.ToDateTime(body.StartTime);
            occurrence.StartDate = ParisTime.ToUtc(start);
            occurrence.EndDate = ParisTime.ToUtc(start + duration);
            return occurrence;
        }).ToList();

        var created = await handler.CreateSeriesAsync(occurrences, dates.ToArray(), ct);
        return Results.Ok(created.Select(M.Event.DomainToContract).ToList());
    }

    /// <summary>GET /api/events/series/{seriesId} : occurrences de la série, par date.</summary>
    private static async Task<IResult> ListAsync(Guid seriesId, [FromServices] H.Event handler, CancellationToken ct)
    {
        var series = await handler.SearchBySeriesAsync(seriesId, ct);
        return series.Data.Count == 0
            ? Results.NotFound()
            : Results.Ok(series.Data.Select(M.Event.DomainToContract).ToList());
    }

    /// <summary>
    /// PUT /api/events/series/{seriesId}?from=2026-11-01 : applique les champs communs (nom, description, âge, organisation,
    /// année, lieu, heure de début et durée) aux occurrences à partir de cette date (toutes si from est omis). Chaque occurrence garde sa date.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid seriesId, DateOnly? from, Ct.EventSeriesUpdate body, [FromServices] H.Event handler, CancellationToken ct)
    {
        if (body.DurationMinutes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(body.DurationMinutes), "Duration must be greater or equal than 0.");
        }

        var targets = await OccurrencesFromAsync(handler, seriesId, from, ct);
        if (targets.Count == 0)
        {
            return Results.NotFound();
        }

        foreach (var occurrence in targets)
        {
            var start = DateOnly.FromDateTime(ParisTime.ToLocal(occurrence.StartDate)).ToDateTime(body.StartTime);
            occurrence.Name = body.Name;
            occurrence.Description = body.Description;
            occurrence.AgeMinimum = body.AgeMinimum;
            occurrence.OrganizationId = body.OrganizationId;
            occurrence.SchoolYearId = body.SchoolYearId;
            occurrence.SpaceId = body.SpaceId;
            occurrence.StartDate = ParisTime.ToUtc(start);
            occurrence.EndDate = ParisTime.ToUtc(start.AddMinutes(body.DurationMinutes));
            await handler.EditAsync(occurrence, ct);
        }

        var updated = await handler.SearchBySeriesAsync(seriesId, ct);
        return Results.Ok(updated.Data.Select(M.Event.DomainToContract).ToList());
    }

    /// <summary>DELETE /api/events/series/{seriesId}?from=2026-11-01 : supprime (logiquement) les occurrences à partir de cette date, ou toutes.</summary>
    private static async Task<IResult> DeleteAsync(Guid seriesId, DateOnly? from, [FromServices] H.Event handler, CancellationToken ct)
    {
        var targets = await OccurrencesFromAsync(handler, seriesId, from, ct);
        if (targets.Count == 0)
        {
            return Results.NotFound();
        }

        foreach (var occurrence in targets)
        {
            await handler.DeleteAsync(occurrence, ct);
        }

        return Results.NoContent();
    }

    private static async Task<List<Do.Event>> OccurrencesFromAsync(H.Event handler, Guid seriesId, DateOnly? from, CancellationToken ct)
    {
        var series = await handler.SearchBySeriesAsync(seriesId, ct);
        return series.Data
            .Where(item => from is null || DateOnly.FromDateTime(ParisTime.ToLocal(item.StartDate)) >= from)
            .ToList();
    }
}
