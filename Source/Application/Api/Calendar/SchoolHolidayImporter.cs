using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Ct = Library.Contract;
using Do = Library.Domain;
using H = Library.Handler;
using M = Library.Mapper;

namespace Api.Calendar;

/// <summary>Le calendrier scolaire national n'a pas pu être lu (réseau, service indisponible, réponse invalide).</summary>
public sealed class SchoolCalendarUnavailableException(Exception inner)
    : Exception("The national school calendar (data.education.gouv.fr) could not be read.", inner);

/// <summary>
/// Import des vacances scolaires officielles d'une année scolaire (code « 2026-2027 ») pour sa zone (A, B ou C),
/// rattachées à cette année. Une période déjà présente (même année, même premier jour) n'est pas recréée.
/// </summary>
public sealed class SchoolHolidayImporter
{
    public const string HttpClientName = "school-holidays";

    private readonly IHttpClientFactory _httpClients;
    private readonly SchoolHolidayOptions _options;
    private readonly H.Holiday _holidays;

    public SchoolHolidayImporter(IHttpClientFactory httpClients, IOptions<SchoolHolidayOptions> options, H.Holiday holidays)
    {
        _httpClients = httpClients;
        _options = options.Value;
        _holidays = holidays;
    }

    public async Task<Ct.HolidayImport> ImportAsync(Do.SchoolYear year, CancellationToken ct)
    {
        // Toutes les académies d'une zone métropolitaine ont les mêmes dates : on dédoublonne.
        var where = $"annee_scolaire=\"{year.Code}\" and zones=\"Zone {year.Zone}\"";
        var url = $"{_options.DatasetUrl}?where={Uri.EscapeDataString(where)}&order_by=start_date&limit=100";

        DatasetResponse? response;
        try
        {
            response = await _httpClients.CreateClient(HttpClientName).GetFromJsonAsync<DatasetResponse>(url, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            throw new SchoolCalendarUnavailableException(ex);
        }

        var existing = (await _holidays.SearchBySchoolYearAsync(year.Id, 0, 0, ct)).Data
            .Select(item => DateOnly.FromDateTime(ParisTime.ToLocal(item.StartDate)))
            .ToHashSet();

        var result = new Ct.HolidayImport { SchoolYearCode = year.Code, Zone = year.Zone };

        // Les élèves seulement (« - » ou « Élèves ») : les dates propres aux enseignants sont ignorées.
        var records = (response?.Results ?? [])
            .Where(r => r.Population is not "Enseignants" && r.StartDate is not null)
            .DistinctBy(r => (r.Description, r.StartDate, r.EndDate));

        foreach (var record in records)
        {
            var (first, last) = Days(record.StartDate!.Value, record.EndDate, record.Description);
            if (!existing.Add(first))
            {
                result.Skipped.Add(record.Description);
                continue;
            }

            var created = await _holidays.CreateAsync(new Do.Holiday
            {
                Name = record.Description,
                StartDate = ParisTime.ToUtc(first.ToDateTime(TimeOnly.MinValue)),
                EndDate = ParisTime.ToUtc(last.ToDateTime(TimeOnly.MinValue)),
                SchoolYearId = year.Id,
                Active = new Do.Active(),
                Audit = new Do.Audit()
            }, ct);

            result.Created.Add(M.Holiday.DomainToContract(created.Data!));
        }

        return result;
    }

    /// <summary>
    /// Jours de vacances : le jeu de données donne le premier jour et le jour de reprise (minuit, heure de Paris) ;
    /// le dernier jour est la veille de la reprise. Pont d'un jour (début = fin) : ce jour seul. « Début des vacances d'été »
    /// sans fin connue : jusqu'au 31 août.
    /// </summary>
    private static (DateOnly First, DateOnly Last) Days(DateTime startUtc, DateTime? endUtc, string description)
    {
        var first = DateOnly.FromDateTime(ParisTime.ToLocal(startUtc));

        if (description.StartsWith("Début", StringComparison.OrdinalIgnoreCase))
        {
            return (first, new DateOnly(first.Year, 8, 31));
        }

        var resume = endUtc is { } end ? DateOnly.FromDateTime(ParisTime.ToLocal(end)) : first;
        var last = resume.AddDays(-1);
        return (first, last < first ? first : last);
    }

    private sealed record DatasetResponse([property: JsonPropertyName("results")] List<DatasetRecord> Results);

    private sealed record DatasetRecord(
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("population")] string? Population,
        [property: JsonPropertyName("start_date")] DateTime? StartDate,
        [property: JsonPropertyName("end_date")] DateTime? EndDate);
}
