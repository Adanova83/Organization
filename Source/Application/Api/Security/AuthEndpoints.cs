using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Api.Security;

/// <summary>
/// Connexion du site web (schéma BFF). La redirection de retour de l'annuaire est traitée par le middleware OIDC
/// sur /api/auth/callback. Le membre connecté s'obtient ensuite par GET /api/me.
/// </summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // Démarre la connexion : redirige vers l'annuaire, puis revient sur returnUrl (adresse relative uniquement).
        group.MapGet("/login", (string? returnUrl, IOptions<WebSessionOptions> session) =>
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = SafeReturnUrl(returnUrl, session.Value.DefaultReturnUrl)
            };

            return Results.Challenge(properties, new[] { AuthSchemes.Oidc });
        });

        // Ferme la session du site (et, si configuré, celle de l'annuaire). En GET : c'est une navigation du navigateur,
        // un cookie ne pouvant pas être supprimé par un appel serveur à serveur. Une déconnexion forcée est sans gravité.
        group.MapGet("/logout", async (string? returnUrl, HttpContext http, IOptions<WebSessionOptions> session) =>
        {
            var target = SafeReturnUrl(returnUrl, session.Value.DefaultReturnUrl);

            if (session.Value.EndProviderSessionOnLogout)
            {
                return Results.SignOut(
                    new AuthenticationProperties { RedirectUri = target },
                    new[] { AuthSchemes.Cookie, AuthSchemes.Oidc });
            }

            await http.SignOutAsync(AuthSchemes.Cookie);
            return Results.LocalRedirect(target);
        });

        return app;
    }

    /// <summary>N'accepte qu'une adresse relative au site (« /chemin ») : pas de « // », de « /\ » ni de caractère de contrôle.</summary>
    internal static string SafeReturnUrl(string? url, string fallback)
    {
        if (string.IsNullOrWhiteSpace(url)
            || url[0] != '/'
            || url.StartsWith("//", StringComparison.Ordinal)
            || url.StartsWith("/\\", StringComparison.Ordinal)
            || url.Any(char.IsControl))
        {
            return fallback;
        }

        return url;
    }
}
