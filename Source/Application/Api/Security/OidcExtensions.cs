using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Api.Security;

public static class OidcExtensions
{
    /// <summary>Vrai si l'API doit mener la connexion OIDC du site web (Oidc:Authority et Oidc:ClientId renseignés).</summary>
    public static bool IsWebLoginEnabled(IConfiguration configuration)
        => (configuration.GetSection(OidcOptions.Section).Get<OidcOptions>() ?? new OidcOptions()).WebLoginEnabled;

    /// <summary>
    /// Enregistre l'authentification si Oidc:Authority est renseigné :
    /// validation des jetons Bearer (clients techniques, Swagger) et, si Oidc:ClientId est renseigné,
    /// connexion du site web par cookie de session (l'API mène le flux OIDC).
    /// Retourne true si l'authentification est active.
    /// </summary>
    public static bool AddOidcAuthentication(
        this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var section = configuration.GetSection(OidcOptions.Section);
        services.Configure<OidcOptions>(section);
        services.Configure<WebSessionOptions>(configuration.GetSection(WebSessionOptions.Section));

        var options = section.Get<OidcOptions>() ?? new OidcOptions();
        if (!options.IsConfigured)
        {
            return false;
        }

        var session = configuration.GetSection(WebSessionOptions.Section).Get<WebSessionOptions>() ?? new WebSessionOptions();
        var webLogin = options.WebLoginEnabled;

        var authentication = services.AddAuthentication(
            webLogin ? AuthSchemes.Smart : JwtBearerDefaults.AuthenticationScheme);

        authentication.AddJwtBearer(jwt =>
        {
            jwt.Authority = options.Authority;
            jwt.RequireHttpsMetadata = options.RequireHttpsMetadata;

            // Garde les noms de claims du jeton ("sub", "email"...) au lieu de les convertir en URI WS-*.
            jwt.MapInboundClaims = false;

            if (string.IsNullOrWhiteSpace(options.Audience))
            {
                jwt.TokenValidationParameters.ValidateAudience = false;
            }
            else
            {
                jwt.Audience = options.Audience;
            }
        });

        if (webLogin)
        {
            // Clés de chiffrement du cookie : à conserver entre deux démarrages (volume), sinon les sessions sautent.
            var dataProtection = services.AddDataProtection().SetApplicationName("Adanova.Organizations.Api");
            var keysPath = configuration["DataProtection:KeysPath"];
            if (!string.IsNullOrWhiteSpace(keysPath))
            {
                dataProtection.PersistKeysToFileSystem(new DirectoryInfo(keysPath));
            }

            // Jeton Bearer présent -> JwtBearer ; sinon -> cookie de session.
            authentication.AddPolicyScheme(AuthSchemes.Smart, "Bearer ou session", policy =>
            {
                policy.ForwardDefaultSelector = context =>
                    context.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                        ? JwtBearerDefaults.AuthenticationScheme
                        : AuthSchemes.Cookie;
            });

            authentication.AddCookie(AuthSchemes.Cookie, cookie =>
            {
                cookie.Cookie.Name = session.CookieName;
                cookie.Cookie.HttpOnly = true;
                cookie.Cookie.SameSite = SameSiteMode.Lax;
                cookie.Cookie.SecurePolicy = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                cookie.ExpireTimeSpan = TimeSpan.FromHours(Math.Max(session.LifetimeHours, 1));
                cookie.SlidingExpiration = false;

                // API : pas de redirection vers une page de connexion, simplement 401 / 403.
                cookie.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                cookie.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

            authentication.AddOpenIdConnect(AuthSchemes.Oidc, oidc =>
            {
                oidc.Authority = options.Authority;
                oidc.RequireHttpsMetadata = options.RequireHttpsMetadata;
                oidc.ClientId = options.ClientId;
                oidc.ClientSecret = options.ClientSecret;

                // Flux « code » + PKCE ; retour par redirection GET (compatible avec un cookie SameSite=Lax).
                oidc.ResponseType = OpenIdConnectResponseType.Code;
                oidc.ResponseMode = OpenIdConnectResponseMode.Query;
                oidc.UsePkce = true;

                oidc.CallbackPath = "/api/auth/callback";
                oidc.SignedOutCallbackPath = "/api/auth/signout-callback";
                oidc.SignInScheme = AuthSchemes.Cookie;

                // Les jetons restent côté serveur : seuls les claims d'identité (sub, email, groups...) vont dans le cookie.
                oidc.SaveTokens = false;
                oidc.GetClaimsFromUserInfoEndpoint = false;
                oidc.MapInboundClaims = false;

                oidc.Scope.Clear();
                foreach (var scope in options.Scopes)
                {
                    oidc.Scope.Add(scope);
                }

                oidc.CorrelationCookie.SameSite = SameSiteMode.Lax;
                oidc.NonceCookie.SameSite = SameSiteMode.Lax;

                oidc.TokenValidationParameters.NameClaimType = "name";
                oidc.TokenValidationParameters.RoleClaimType = options.GroupsClaim;
            });
        }

        services.AddAuthorization();
        return true;
    }
}
