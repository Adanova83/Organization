using Microsoft.Extensions.Options;
using Web.Security;

namespace Web.Clients;

/// <summary>
/// Ajoute à chaque appel vers l'API le cookie de session du navigateur (et lui seul), plus l'en-tête anti-CSRF
/// exigé par l'API sur les requêtes qui modifient des données.
/// </summary>
public sealed class ApiCookieRelayHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _accessor;
    private readonly string _cookieName;

    public ApiCookieRelayHandler(IHttpContextAccessor accessor, IOptions<SessionSettings> settings)
    {
        _accessor = accessor;
        _cookieName = settings.Value.CookieName;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var http = _accessor.HttpContext;

        if (http is not null
            && http.Request.Cookies.TryGetValue(_cookieName, out var session)
            && !string.IsNullOrEmpty(session))
        {
            request.Headers.TryAddWithoutValidation("Cookie", $"{_cookieName}={session}");
        }

        request.Headers.TryAddWithoutValidation("X-Csrf", "1");

        return base.SendAsync(request, cancellationToken);
    }
}
