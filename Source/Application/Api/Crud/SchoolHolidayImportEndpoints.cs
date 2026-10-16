using Api.Calendar;
using Api.Security;
using Microsoft.AspNetCore.Mvc;
using Do = Library.Domain;
using H = Library.Handler;

namespace Api.Crud;

/// <summary>Import des vacances scolaires officielles d'une année scolaire, pour sa zone (permission organization.edit).</summary>
public static class SchoolHolidayImportEndpoints
{
    public static IServiceCollection AddSchoolHolidayImport(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SchoolHolidayOptions>(configuration.GetSection(SchoolHolidayOptions.Section));
        services.AddHttpClient(SchoolHolidayImporter.HttpClientName, client => client.Timeout = TimeSpan.FromSeconds(20));
        services.AddScoped<SchoolHolidayImporter>();
        return services;
    }

    /// <summary>POST /api/school-years/{id}/import-holidays : (ré)importe les vacances de l'année ; les périodes déjà présentes sont ignorées.</summary>
    public static RouteGroupBuilder MapSchoolHolidayImport(this RouteGroupBuilder schoolYears)
    {
        schoolYears.MapPost("/{id:guid}/import-holidays", ImportAsync).RequirePermission(Permissions.OrganizationEdit);
        return schoolYears;
    }

    /// <summary>
    /// Import automatique après la création d'une année. Un échec (calendrier indisponible, année pas encore publiée)
    /// n'empêche pas la création : il est journalisé, et l'import peut être relancé depuis la fiche de l'année.
    /// </summary>
    public static async Task ImportAfterCreateAsync(Do.SchoolYear year, IServiceProvider services, CancellationToken ct)
    {
        try
        {
            var result = await services.GetRequiredService<SchoolHolidayImporter>().ImportAsync(year, ct);
            services.GetRequiredService<ILogger<SchoolHolidayImporter>>()
                .LogInformation("School year {Code}: {Count} school holiday period(s) imported for zone {Zone}.", year.Code, result.Created.Count, year.Zone);
        }
        catch (SchoolCalendarUnavailableException ex)
        {
            services.GetRequiredService<ILogger<SchoolHolidayImporter>>()
                .LogWarning(ex, "School year {Code}: school holidays could not be imported.", year.Code);
        }
    }

    private static async Task<IResult> ImportAsync(
        Guid id, [FromServices] H.SchoolYear schoolYears, [FromServices] SchoolHolidayImporter importer, CancellationToken ct)
    {
        var year = (await schoolYears.SearchAsync(id, ct)).Data;
        if (year is null)
        {
            return Results.NotFound();
        }

        try
        {
            return Results.Ok(await importer.ImportAsync(year, ct));
        }
        catch (SchoolCalendarUnavailableException)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "School calendar unavailable.",
                detail: "The national school calendar (data.education.gouv.fr) could not be read. Try again later.");
        }
    }
}
