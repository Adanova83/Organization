using Microsoft.AspNetCore.Authentication;
using Web.Clients;
using Web.Components;
using Web.Security;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Adresse de l'API (réseau interne). Le site ne parle qu'à l'API, jamais à l'annuaire.
var apiBase = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Api:BaseUrl must be set.");
if (!apiBase.EndsWith('/'))
{
    apiBase += "/";
}

builder.Services.AddRazorComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<UserAccess>();
builder.Services.AddTransient<SubscriptionClient>();
builder.Services.AddTransient<AnimationClient>();
builder.Services.AddHealthChecks();
builder.Services.Configure<SessionSettings>(builder.Configuration.GetSection("Session"));

// Client typé de l'API : relaie le cookie de session du navigateur (rendu côté serveur : une requête = un HttpContext).
builder.Services.AddTransient<ApiCookieRelayHandler>();
builder.Services.AddHttpClient<ApiClient>(client => client.BaseAddress = new Uri(apiBase))
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        // Jamais de cookies conservés par le client : ils seraient partagés entre utilisateurs.
        UseCookies = false,
        AllowAutoRedirect = false
    })
    .AddHttpMessageHandler<ApiCookieRelayHandler>();

// L'utilisateur est celui de la session ouverte par l'API (GET /api/me).
builder.Services
    .AddAuthentication(ApiSessionDefaults.Scheme)
    .AddScheme<AuthenticationSchemeOptions, ApiSessionAuthenticationHandler>(ApiSessionDefaults.Scheme, null);

builder.Services.AddAuthorization(options =>
{
    // Une politique par permission (member.read, activity.edit...), à utiliser avec [Authorize(Policy = ...)] ou <AuthorizeView Policy="...">.
    foreach (var permission in Permissions.All)
    {
        options.AddPolicy(permission, policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context => Permissions.Has(context.User, permission)));
    }
});

// Les adresses /api/* (connexion, callback OIDC, API) sont transmises à l'API : une seule origine pour le navigateur,
// donc un cookie de même origine, sans CORS. En production, Traefik peut aussi router /api directement vers l'API.
builder.Services.AddReverseProxy().LoadFromMemory(
    new[]
    {
        new RouteConfig
        {
            RouteId = "api",
            ClusterId = "api",
            Match = new RouteMatch { Path = "/api/{**catch-all}" },
            // Conserve l'hôte public : l'API en déduit l'adresse de retour OIDC (/api/auth/callback).
            Transforms = new[] { new Dictionary<string, string> { ["RequestHeaderOriginalHost"] = "true" } }
        }
    },
    new[]
    {
        new ClusterConfig
        {
            ClusterId = "api",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["api"] = new DestinationConfig { Address = apiBase }
            }
        }
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapHealthChecks("/health");
app.MapStaticAssets();
app.MapReverseProxy();
app.MapRazorComponents<App>();

app.Run();
