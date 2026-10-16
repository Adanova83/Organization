using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Web.Clients;

namespace Web.Security;

public static class ApiSessionDefaults
{
    public const string Scheme = "ApiSession";
}

/// <summary>
/// Authentifie l'utilisateur d'après la session ouverte par l'API : le cookie du navigateur est relayé à GET /api/me,
/// qui renvoie le membre, ses groupes et ses permissions. Le site ne voit jamais de jeton. Le résultat est gardé
/// quelques secondes en mémoire (clé : empreinte du cookie) pour ne pas interroger l'API à chaque requête.
/// Sans session, ou si elle a expiré, le défi redirige vers /api/auth/login.
/// </summary>
public sealed class ApiSessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    private readonly ApiClient _api;
    private readonly IMemoryCache _cache;
    private readonly string _cookieName;

    public ApiSessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ApiClient api,
        IMemoryCache cache,
        IOptions<SessionSettings> settings)
        : base(options, logger, encoder)
    {
        _api = api;
        _cache = cache;
        _cookieName = settings.Value.CookieName;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Les appels /api/* sont transmis à l'API, qui authentifie elle-même.
        if (Request.Path.StartsWithSegments("/api")
            || !Request.Cookies.TryGetValue(_cookieName, out var session)
            || string.IsNullOrEmpty(session))
        {
            return AuthenticateResult.NoResult();
        }

        var key = CacheKey(session);

        if (!_cache.TryGetValue(key, out MeResult? result) || result is null)
        {
            result = await _api.GetMeAsync(Context.RequestAborted);

            if (result.Status is MeStatus.Ok or MeStatus.NoMember)
            {
                _cache.Set(key, result, CacheDuration);
            }
        }

        return result.Status switch
        {
            MeStatus.Ok => Success(BuildPrincipal(result.Me!)),
            MeStatus.NoMember => Success(BuildPrincipalWithoutMember()),
            MeStatus.Unauthorized => AuthenticateResult.NoResult(),
            _ => AuthenticateResult.Fail("L'API est injoignable ou a renvoyé une erreur.")
        };
    }

    /// <summary>Clé du cache de GET /api/me pour une session (à retirer après une modification du profil).</summary>
    public static string CacheKey(string session)
        => "me:" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(session)));

    private AuthenticateResult Success(ClaimsPrincipal principal)
        => AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));

    private ClaimsPrincipal BuildPrincipal(MeResponse me)
    {
        var name = $"{me.Member.Firstname} {me.Member.Lastname}".Trim();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, me.Member.Email),
            new("member_id", me.Member.Id.ToString()),
            new("member_linked", "true")
        };

        if (me.Member.Birthdate is { } birthdate)
        {
            claims.Add(new Claim("birthdate", birthdate.ToUniversalTime().ToString("O")));
        }

        claims.AddRange(me.Groups.Select(group => new Claim(ClaimTypes.Role, group)));
        claims.AddRange(me.Permissions.Select(permission => new Claim(Permissions.ClaimType, permission)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
    }

    /// <summary>Compte connecté à l'annuaire mais sans membre : connecté, sans aucune permission.</summary>
    private ClaimsPrincipal BuildPrincipalWithoutMember()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Compte sans membre"),
            new("member_linked", "false")
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var returnUrl = Uri.EscapeDataString(Request.PathBase + Request.Path + Request.QueryString);
        Response.Redirect($"/api/auth/login?returnUrl={returnUrl}");
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/access-denied");
        return Task.CompletedTask;
    }
}
