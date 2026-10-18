using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ct = Library.Contract;

namespace Web.Clients;

/// <summary>Client typé de l'API. Les droits sont vérifiés par l'API, avec la session de l'utilisateur.</summary>
public sealed class ApiClient
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    /// <summary>Membre connecté, rôles et permissions (GET /api/me).</summary>
    public async Task<MeResult> GetMeAsync(CancellationToken ct = default)
    {
        try
        {
            using var response = await _http.GetAsync("api/me", ct);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var me = await response.Content.ReadFromJsonAsync<MeResponse>(Json, ct);
                    return me is null ? new MeResult(MeStatus.Error) : new MeResult(MeStatus.Ok, me);
                case HttpStatusCode.Unauthorized:
                    return new MeResult(MeStatus.Unauthorized);
                case HttpStatusCode.Forbidden:
                    return new MeResult(MeStatus.NoMember);
                default:
                    return new MeResult(MeStatus.Error);
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            return new MeResult(MeStatus.Error);
        }
    }

    public Task<PagedResponse<Ct.Member>> GetMembersAsync(string? query, int page, int pageSize, CancellationToken ct = default)
    {
        var url = $"api/members?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(query))
        {
            url += "&query=" + Uri.EscapeDataString(query);
        }

        return GetAsync<PagedResponse<Ct.Member>>(url, ct);
    }

    // ---- Accès générique (routes de l'API, voir ApiRoutes) ----

    public Task<T> GetByIdAsync<T>(string route, Guid id, CancellationToken ct = default)
        => GetAsync<T>($"{route}/{id}", ct);

    /// <summary>Liste simple (non paginée) renvoyée par une route.</summary>
    public Task<List<T>> GetListAsync<T>(string url, CancellationToken ct = default)
        => GetAsync<List<T>>(url, ct);

    public Task<T> CreateAsync<T>(string route, T body, CancellationToken ct = default) where T : notnull
        => SendAsync<T>(HttpMethod.Post, route, body, ct);

    public Task<T> UpdateAsync<T>(string route, Guid id, T body, CancellationToken ct = default) where T : notnull
        => SendAsync<T>(HttpMethod.Put, $"{route}/{id}", body, ct);

    public Task DeleteAsync(string route, Guid id, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, $"{route}/{id}", ct);

    /// <summary>Toute une liste (200 éléments par page), éventuellement filtrée par ?targetId= et ?memberId=.</summary>
    public Task<List<T>> ListAllAsync<T>(string route, Guid? targetId = null, Guid? memberId = null, CancellationToken ct = default)
    {
        var filters = new List<string>();
        if (targetId is { } target) filters.Add($"targetId={target}");
        if (memberId is { } member) filters.Add($"memberId={member}");

        return GetAllAsync<T>(filters.Count == 0 ? route : $"{route}?{string.Join('&', filters)}", ct);
    }

    /// <summary>Une page d'une liste, avec recherche facultative (?query=).</summary>
    public Task<PagedResponse<T>> GetPageAsync<T>(string route, string? query, int page, int pageSize, CancellationToken ct = default)
        => GetPagedAsync<T>(route, query, page, pageSize, ct);

    /// <summary>Modifie le profil du membre connecté (PUT /api/me, sans permission particulière).</summary>
    public Task<Ct.Member> UpdateMeAsync(ProfileUpdate profile, CancellationToken ct = default)
        => SendAsync<Ct.Member>(HttpMethod.Put, ApiRoutes.Me, profile, ct);

    /// <summary>Crée les jours fériés légaux d'une année (POST /api/holidays/generate, organization.edit). Les jours déjà présents sont ignorés.</summary>
    public Task<Ct.HolidayGeneration> GenerateHolidaysAsync(int year, bool alsaceMoselle, CancellationToken ct = default)
        => SendAsync<Ct.HolidayGeneration>(
            HttpMethod.Post, $"{ApiRoutes.Holidays}/generate?year={year}&alsaceMoselle={(alsaceMoselle ? "true" : "false")}", new { }, ct);

    /// <summary>Tous les membres, triés par nom. Vide si l'utilisateur n'a pas member.read (les pages affichent alors un libellé neutre).</summary>
    public async Task<List<Ct.Member>> GetMembersOrEmptyAsync(CancellationToken ct = default)
    {
        try
        {
            return (await GetAllAsync<Ct.Member>(ApiRoutes.Members, ct))
                .OrderBy(m => m.Lastname).ThenBy(m => m.Firstname).ToList();
        }
        catch (ApiException ex) when (ex.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
        {
            return [];
        }
    }

    /// <summary>Importe les vacances scolaires officielles d'une année (zone et académie configurées dans l'API, organization.edit).</summary>
    public Task<Ct.HolidayImport> ImportSchoolHolidaysAsync(Guid schoolYearId, CancellationToken ct = default)
        => SendAsync<Ct.HolidayImport>(HttpMethod.Post, $"{ApiRoutes.SchoolYears}/{schoolYearId}/import-holidays", new { }, ct);

    /// <summary>Années scolaires, la plus récente d'abord (organization.read).</summary>
    public Task<List<Ct.SchoolYear>> GetSchoolYearsAsync(CancellationToken ct = default)
        => GetAllAsync<Ct.SchoolYear>(ApiRoutes.SchoolYears, ct);

    public Task<List<Ct.Organization>> GetOrganizationsAsync(CancellationToken ct = default)
        => GetAllAsync<Ct.Organization>(ApiRoutes.Organizations, ct);

    public Task<List<Ct.OrganizationSession>> GetOrganizationSessionsAsync(Guid organizationId, CancellationToken ct = default)
        => GetAllAsync<Ct.OrganizationSession>($"{ApiRoutes.Organizations}/{organizationId}/sessions", ct);

    /// <summary>Cotisations d'une saison par membre (contribution.read).</summary>
    public Task<Ct.SeasonContributions> GetSeasonContributionsAsync(Guid organizationSessionId, CancellationToken ct = default)
        => GetAsync<Ct.SeasonContributions>($"{ApiRoutes.Contributions}/seasons/{organizationSessionId}", ct);

    /// <summary>Espaces. Vide si l'utilisateur n'a pas space.read.</summary>
    public async Task<List<Ct.Space>> GetSpacesAsync(CancellationToken ct = default)
    {
        try
        {
            return await GetAllAsync<Ct.Space>(ApiRoutes.Spaces, ct);
        }
        catch (ApiException ex) when (ex.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
        {
            return [];
        }
    }

    // ---- Planning : activités, événements, projets ----

    public Task<PagedResponse<Ct.Activity>> GetActivitiesAsync(string? query, int page, int pageSize, CancellationToken ct = default)
        => GetPagedAsync<Ct.Activity>("api/activities", query, page, pageSize, ct);

    public Task<Ct.Activity> GetActivityAsync(Guid id, CancellationToken ct = default)
        => GetAsync<Ct.Activity>($"api/activities/{id}", ct);

    public Task<List<Ct.ActivitySession>> GetActivitySessionsAsync(Guid activityId, CancellationToken ct = default)
        => GetAllAsync<Ct.ActivitySession>($"api/activities/{activityId}/sessions", ct);

    public Task<List<Ct.ActivitySessionSlot>> GetActivitySessionSlotsAsync(Guid sessionId, CancellationToken ct = default)
        => GetAllAsync<Ct.ActivitySessionSlot>($"api/activity-sessions/{sessionId}/slots", ct);

    public Task<PagedResponse<Ct.Event>> GetEventsAsync(string? query, int page, int pageSize, CancellationToken ct = default)
        => GetPagedAsync<Ct.Event>("api/events", query, page, pageSize, ct);

    public Task<Ct.Event> GetEventAsync(Guid id, CancellationToken ct = default)
        => GetAsync<Ct.Event>($"api/events/{id}", ct);

    // ---- Séries d'événements récurrents ----

    /// <summary>Crée une série : une occurrence par date de la récurrence (event.edit).</summary>
    public Task<List<Ct.Event>> CreateEventSeriesAsync(Ct.EventSeries series, CancellationToken ct = default)
        => SendAsync<List<Ct.Event>>(HttpMethod.Post, $"{ApiRoutes.Events}/series", series, ct);

    public Task<List<Ct.Event>> GetEventSeriesAsync(Guid seriesId, CancellationToken ct = default)
        => GetAsync<List<Ct.Event>>($"{ApiRoutes.Events}/series/{seriesId}", ct);

    /// <summary>Applique les champs communs aux occurrences à partir de <paramref name="from"/> (toutes si null).</summary>
    public Task<List<Ct.Event>> UpdateEventSeriesAsync(Guid seriesId, DateOnly? from, Ct.EventSeriesUpdate update, CancellationToken ct = default)
        => SendAsync<List<Ct.Event>>(HttpMethod.Put, SeriesUrl(seriesId, from), update, ct);

    /// <summary>Supprime les occurrences à partir de <paramref name="from"/> (toutes si null).</summary>
    public Task DeleteEventSeriesAsync(Guid seriesId, DateOnly? from, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, SeriesUrl(seriesId, from), ct);

    private static string SeriesUrl(Guid seriesId, DateOnly? from)
        => $"{ApiRoutes.Events}/series/{seriesId}" + (from is { } date ? $"?from={date:yyyy-MM-dd}" : "");

    // ---- Présences (feuilles d'émargement) des ateliers ----

    public Task<Ct.AttendanceOccurrences> GetAttendanceOccurrencesAsync(Guid slotId, CancellationToken ct = default)
        => GetAsync<Ct.AttendanceOccurrences>($"api/attendances/slots/{slotId}", ct);

    public Task<Ct.AttendanceSheet> GetAttendanceSheetAsync(Guid slotId, DateOnly date, CancellationToken ct = default)
        => GetAsync<Ct.AttendanceSheet>($"api/attendances/slots/{slotId}/{date:yyyy-MM-dd}", ct);

    /// <summary>Enregistre l'appel et renvoie la feuille à jour.</summary>
    public Task<Ct.AttendanceSheet> RecordAttendanceAsync(Guid slotId, DateOnly date, List<Ct.AttendanceEntry> entries, CancellationToken ct = default)
        => SendAsync<Ct.AttendanceSheet>(HttpMethod.Put, $"api/attendances/slots/{slotId}/{date:yyyy-MM-dd}", entries, ct);

    public Task<Ct.AttendanceStats> GetAttendanceStatsAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
        => GetAsync<Ct.AttendanceStats>($"api/attendances/stats?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}", ct);

    public Task<PagedResponse<Ct.Project>> GetProjectsAsync(string? query, int page, int pageSize, CancellationToken ct = default)
        => GetPagedAsync<Ct.Project>("api/projects", query, page, pageSize, ct);

    public Task<Ct.Project> GetProjectAsync(Guid id, CancellationToken ct = default)
        => GetAsync<Ct.Project>($"api/projects/{id}", ct);

    public Task<List<Ct.ProjectSlot>> GetProjectSlotsAsync(Guid projectId, CancellationToken ct = default)
        => GetAllAsync<Ct.ProjectSlot>($"api/projects/{projectId}/slots", ct);

    /// <summary>Nom des espaces par identifiant (vide sans space.read : l'affichage se passe alors du lieu).</summary>
    public async Task<Dictionary<Guid, string>> GetSpaceNamesAsync(CancellationToken ct = default)
        => (await GetSpacesAsync(ct)).ToDictionary(space => space.Id, space => space.Name);

    public Task<Ct.Member> GetMemberAsync(Guid id, CancellationToken ct = default)
        => GetAsync<Ct.Member>($"api/members/{id}", ct);

    public Task<Ct.Member> CreateMemberAsync(Ct.Member member, CancellationToken ct = default)
        => SendAsync<Ct.Member>(HttpMethod.Post, "api/members", member, ct);

    public Task<Ct.Member> UpdateMemberAsync(Guid id, Ct.Member member, CancellationToken ct = default)
        => SendAsync<Ct.Member>(HttpMethod.Put, $"api/members/{id}", member, ct);

    public Task DeleteMemberAsync(Guid id, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, $"api/members/{id}", ct);

    /// <summary>Tous les pays (l'API pagine par 200 au plus).</summary>
    public async Task<IReadOnlyList<Ct.Country>> GetCountriesAsync(CancellationToken ct = default)
    {
        var countries = new List<Ct.Country>();

        for (var page = 1; ; page++)
        {
            var result = await GetAsync<PagedResponse<Ct.Country>>($"api/countries?page={page}&pageSize=200", ct);
            countries.AddRange(result.Data);

            if (result.Data.Count == 0 || countries.Count >= result.FilteredCount)
            {
                return countries;
            }
        }
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string url, object body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, url) { Content = JsonContent.Create(body, options: Json) };
        using var response = await _http.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.FromAsync(response, ct);
        }

        return (await response.Content.ReadFromJsonAsync<T>(Json, ct))!;
    }

    private async Task SendAsync(HttpMethod method, string url, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, url);
        using var response = await _http.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.FromAsync(response, ct);
        }
    }

    private Task<PagedResponse<T>> GetPagedAsync<T>(string path, string? query, int page, int pageSize, CancellationToken ct)
    {
        var url = $"{path}?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(query))
        {
            url += "&query=" + Uri.EscapeDataString(query);
        }

        return GetAsync<PagedResponse<T>>(url, ct);
    }

    /// <summary>Parcourt toutes les pages (200 éléments au plus par page) d'une liste.</summary>
    private async Task<List<T>> GetAllAsync<T>(string path, CancellationToken ct)
    {
        var items = new List<T>();

        for (var page = 1; ; page++)
        {
            var result = await GetAsync<PagedResponse<T>>($"{path}{(path.Contains('?') ? '&' : '?')}page={page}&pageSize=200", ct);
            items.AddRange(result.Data);

            if (result.Data.Count == 0 || items.Count >= result.FilteredCount)
            {
                return items;
            }
        }
    }

    private async Task<T> GetAsync<T>(string url, CancellationToken ct)
    {
        using var response = await _http.GetAsync(url, ct);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.FromAsync(response, ct);
        }

        return (await response.Content.ReadFromJsonAsync<T>(Json, ct))!;
    }
}
