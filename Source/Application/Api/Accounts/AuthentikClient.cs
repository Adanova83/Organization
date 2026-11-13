using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Api.Accounts;

/// <summary>Compte de l'annuaire, tel que l'intranet en a besoin.</summary>
public sealed record DirectoryUser(
    int Id, string Username, string Name, string Email, bool IsActive, bool IsSuperuser, IReadOnlyList<string> GroupIds);

/// <summary>Erreur de l'annuaire (injoignable, droits insuffisants, refus de la requête).</summary>
public sealed class DirectoryException(string message, Exception? inner = null) : Exception(message, inner);

/// <summary>Opérations de l'annuaire utilisées par l'intranet (Authentik en production, faux annuaire dans les tests).</summary>
public interface IDirectoryClient
{
    Task<DirectoryUser?> GetUserAsync(int id, CancellationToken ct);
    Task<DirectoryUser?> FindUserByEmailAsync(string email, CancellationToken ct);
    Task<bool> UsernameExistsAsync(string username, CancellationToken ct);
    Task<DirectoryUser> CreateUserAsync(string username, string name, string email, string path, CancellationToken ct);
    Task UpdateUserAsync(int id, string name, string email, CancellationToken ct);
    Task SetActiveAsync(int id, bool isActive, CancellationToken ct);
    Task SetPasswordAsync(int id, string password, CancellationToken ct);
    Task<string> GetGroupIdAsync(string name, CancellationToken ct);
    Task AddToGroupAsync(string groupId, int userId, CancellationToken ct);
    Task RemoveFromGroupAsync(string groupId, int userId, CancellationToken ct);
}

/// <summary>API REST d'Authentik (/api/v3), avec le jeton d'un compte de service.</summary>
public sealed class AuthentikClient : IDirectoryClient
{
    private readonly HttpClient _http;

    public AuthentikClient(HttpClient http, IOptions<DirectoryOptions> options)
    {
        _http = http;
        var settings = options.Value;
        if (settings.IsConfigured)
        {
            _http.BaseAddress = new Uri(settings.BaseUrl!.TrimEnd('/') + "/api/v3/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.Token);
        }
    }

    public async Task<DirectoryUser?> GetUserAsync(int id, CancellationToken ct)
    {
        using var response = await SendAsync(HttpMethod.Get, $"core/users/{id}/", null, ct, allowNotFound: true);
        return response.StatusCode == HttpStatusCode.NotFound ? null : ToUser(await ReadAsync(response, ct));
    }

    public async Task<DirectoryUser?> FindUserByEmailAsync(string email, CancellationToken ct)
    {
        using var response = await SendAsync(HttpMethod.Get, $"core/users/?email={Uri.EscapeDataString(email)}", null, ct);
        var results = (await ReadAsync(response, ct)).GetProperty("results");
        // Le filtre d'Authentik est exact mais sensible à la casse selon la base : on compare nous-mêmes.
        foreach (var item in results.EnumerateArray())
        {
            if (string.Equals(item.GetProperty("email").GetString(), email, StringComparison.OrdinalIgnoreCase))
            {
                return ToUser(item);
            }
        }

        return null;
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken ct)
    {
        using var response = await SendAsync(HttpMethod.Get, $"core/users/?username={Uri.EscapeDataString(username)}&include_groups=false", null, ct);
        return (await ReadAsync(response, ct)).GetProperty("results").GetArrayLength() > 0;
    }

    public async Task<DirectoryUser> CreateUserAsync(string username, string name, string email, string path, CancellationToken ct)
    {
        var body = new { username, name, email, path, is_active = true, type = "internal" };
        using var response = await SendAsync(HttpMethod.Post, "core/users/", body, ct);
        return ToUser(await ReadAsync(response, ct));
    }

    public async Task UpdateUserAsync(int id, string name, string email, CancellationToken ct)
    {
        using var _ = await SendAsync(HttpMethod.Patch, $"core/users/{id}/", new { name, email }, ct);
    }

    public async Task SetActiveAsync(int id, bool isActive, CancellationToken ct)
    {
        using var _ = await SendAsync(HttpMethod.Patch, $"core/users/{id}/", new { is_active = isActive }, ct);
    }

    public async Task SetPasswordAsync(int id, string password, CancellationToken ct)
    {
        using var _ = await SendAsync(HttpMethod.Post, $"core/users/{id}/set_password/", new { password }, ct);
    }

    public async Task<string> GetGroupIdAsync(string name, CancellationToken ct)
    {
        using var response = await SendAsync(HttpMethod.Get, $"core/groups/?name={Uri.EscapeDataString(name)}&include_users=false", null, ct);
        var results = (await ReadAsync(response, ct)).GetProperty("results");
        if (results.GetArrayLength() == 0)
        {
            throw new DirectoryException($"Le groupe « {name} » n'existe pas dans Authentik.");
        }

        return results[0].GetProperty("pk").GetString()!;
    }

    public async Task AddToGroupAsync(string groupId, int userId, CancellationToken ct)
    {
        using var _ = await SendAsync(HttpMethod.Post, $"core/groups/{groupId}/add_user/", new { pk = userId }, ct);
    }

    public async Task RemoveFromGroupAsync(string groupId, int userId, CancellationToken ct)
    {
        using var _ = await SendAsync(HttpMethod.Post, $"core/groups/{groupId}/remove_user/", new { pk = userId }, ct);
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body, CancellationToken ct, bool allowNotFound = false)
    {
        if (_http.BaseAddress is null)
        {
            throw new DirectoryException("La liaison avec Authentik n'est pas configurée.");
        }

        HttpResponseMessage response;
        try
        {
            using var request = new HttpRequestMessage(method, url);
            if (body is not null)
            {
                // Corps complet avec sa longueur : Authentik ne lit pas un corps envoyé par morceaux (JsonContent).
                request.Content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
            }

            response = await _http.SendAsync(request, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            throw new DirectoryException("Authentik ne répond pas.", ex);
        }

        if (response.IsSuccessStatusCode || (allowNotFound && response.StatusCode == HttpStatusCode.NotFound))
        {
            return response;
        }

        var detail = await response.Content.ReadAsStringAsync(ct);
        var status = (int)response.StatusCode;
        response.Dispose();
        throw new DirectoryException(status switch
        {
            401 or 403 => "Authentik refuse l'accès : vérifiez le jeton et les droits du compte de service de l'intranet.",
            _ => $"Authentik a refusé la demande ({status}) : {Shorten(detail)}"
        });
    }

    private static async Task<JsonElement> ReadAsync(HttpResponseMessage response, CancellationToken ct)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        return document.RootElement.Clone();
    }

    private static DirectoryUser ToUser(JsonElement item)
    {
        var groups = item.TryGetProperty("groups", out var list) && list.ValueKind == JsonValueKind.Array
            ? list.EnumerateArray().Select(group => group.GetString()!).ToList()
            : [];

        return new DirectoryUser(
            item.GetProperty("pk").GetInt32(),
            item.GetProperty("username").GetString() ?? "",
            item.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "",
            item.TryGetProperty("email", out var email) ? email.GetString() ?? "" : "",
            item.TryGetProperty("is_active", out var active) && active.GetBoolean(),
            item.TryGetProperty("is_superuser", out var superuser) && superuser.GetBoolean(),
            groups);
    }

    private static string Shorten(string text) => text.Length <= 300 ? text : text[..300] + "…";
}
