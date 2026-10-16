using Api.Calendar;
using Api.Security;
using Microsoft.AspNetCore.Mvc;
using Ct = Library.Contract;
using Do = Library.Domain;
using H = Library.Handler;
using M = Library.Mapper;

namespace Api.Crud;

public static class HolidayGenerationEndpoints
{
    /// <summary>
    /// POST /api/holidays/generate?year=2027&amp;alsaceMoselle=false : crée les jours fériés légaux de l'année (permission organization.edit).
    /// Un jour déjà présent à la même date (quel que soit son nom) n'est pas recréé : l'appel peut être relancé sans doublon.
    /// Les dates sont enregistrées à minuit, heure de Paris, comme celles saisies dans le site.
    /// </summary>
    public static RouteGroupBuilder MapHolidayGeneration(this RouteGroupBuilder holidays)
    {
        holidays.MapPost("/generate", GenerateAsync).RequirePermission(Permissions.OrganizationEdit);
        return holidays;
    }

    private static async Task<IResult> GenerateAsync(
        int year, bool? alsaceMoselle, [FromServices] H.Holiday handler, CancellationToken ct)
    {
        var rules = Do.FrenchPublicHolidays.For(year, alsaceMoselle ?? false);

        var existing = await handler.SearchAsync(null, 0, 0, ct);
        var takenDays = existing.Data
            .Select(item => DateOnly.FromDateTime(ParisTime.ToLocal(item.StartDate)))
            .ToHashSet();

        var result = new Ct.HolidayGeneration { Year = year };

        foreach (var rule in rules)
        {
            if (takenDays.Contains(rule.Date))
            {
                result.Skipped.Add(rule.Name);
                continue;
            }

            var midnight = ParisTime.ToUtc(rule.Date.ToDateTime(TimeOnly.MinValue));
            var created = await handler.CreateAsync(new Do.Holiday
            {
                Name = rule.Name,
                StartDate = midnight,
                EndDate = midnight,
                Active = new Do.Active(),
                Audit = new Do.Audit()
            }, ct);

            result.Created.Add(M.Holiday.DomainToContract(created.Data!));
        }

        return Results.Ok(result);
    }
}
