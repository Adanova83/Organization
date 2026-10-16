using System.Net;
using System.Text.Json;

namespace Web.Clients;

/// <summary>Erreur renvoyée par l'API (ProblemDetails : title / detail).</summary>
public sealed class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public static async Task<ApiException> FromAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var message = $"Erreur {(int)response.StatusCode} de l'API.";

        try
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;

            var title = root.TryGetProperty("title", out var t) ? t.GetString() : null;
            var detail = root.TryGetProperty("detail", out var d) ? d.GetString() : null;

            if (!string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(detail))
            {
                message = string.Join(" ", new[] { title, detail }.Where(s => !string.IsNullOrWhiteSpace(s)));
            }
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            // Corps vide ou non JSON : on garde le message par défaut.
        }

        return new ApiException(response.StatusCode, message);
    }
}
