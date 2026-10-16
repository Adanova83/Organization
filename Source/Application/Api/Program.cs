using System.Text.Json.Serialization;
using Api;
using Api.Calendar;
using Api.Catalog;
using Api.Crud;
using Api.OpenApi;
using Api.Security;
using Microsoft.EntityFrameworkCore;
using D = Library.Data;

// Charge un éventuel fichier .env (DB_HOST, DB_PORT, DB_NAME, DB_USER, DB_PASSWORD, API_KEY).
// Dans Docker il n'y en a pas : les variables viennent de docker-compose.
var envFile = FindEnvFile();
if (envFile is not null)
{
    DotNetEnv.Env.Load(envFile);
}

var builder = WebApplication.CreateBuilder(args);

// API_KEY (variable d'environnement / .env) alimente Api:Key
var apiKey = Environment.GetEnvironmentVariable("API_KEY");
if (!string.IsNullOrWhiteSpace(apiKey))
{
    builder.Configuration["Api:Key"] = apiKey;
}

var connectionString = ConnectionStrings.Get(builder.Configuration, "Adanova");

builder.Services.AddDbContext<D.FullDbContext>(options =>
    options
        .UseNpgsql(
            connectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(60);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
                // Même table d'historique que l'ancienne application Migration
                npgsqlOptions.MigrationsHistoryTable("MigrationsHistory", "Migration");
            })
        .EnableDetailedErrors());

builder.Services.AddLibraryServices();
builder.Services.AddScoped<MigrationCatalog>();

// Authentification par jeton OIDC (annuaire du domaine) : active uniquement si Oidc:Authority est renseigné.
var oidcEnabled = builder.Services.AddOidcAuthentication(builder.Configuration, builder.Environment);
// Connexion du site web (cookie de session) : active si Oidc:ClientId est renseigné en plus d'Oidc:Authority.
var webLoginEnabled = oidcEnabled && OidcExtensions.IsWebLoginEnabled(builder.Configuration);

// Flux ICS du planning, auxquels Nextcloud s'abonne : actifs uniquement si Calendar:FeedToken est renseigné.
builder.Services.AddCalendarFeeds(builder.Configuration);

// Import des vacances scolaires officielles (data.education.gouv.fr), zone et académie dans la section SchoolHolidays.
builder.Services.AddSchoolHolidayImport(builder.Configuration);

// Rôles = groupes de l'annuaire (claim « groups ») ; permissions = correspondance groupe -> permissions dans la configuration.
builder.Services.AddPermissions(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<ApiDocumentTransformer>());
builder.Services.AddHealthChecks();

var app = builder.Build();

await app.ApplyMigrationsAtStartupAsync();
await app.SeedAdminMemberAsync();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Documentation : /swagger (interface) et /openapi/v1.json (description). Active en Development, ou si Api:EnableSwagger = true.
// À désactiver en production : elle décrit toute l'API.
if (app.Configuration.GetValue("Api:EnableSwagger", app.Environment.IsDevelopment()))
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Organizations API v1");
        options.RoutePrefix = "swagger";
        options.EnablePersistAuthorization();
    });
}

if (!app.Environment.IsDevelopment() && app.Configuration.GetValue("Api:UseHttpsRedirection", true))
{
    // Désactivé dans Docker (Api__UseHttpsRedirection=false) : le TLS est géré par un reverse proxy.
    app.UseHttpsRedirection();
}

if (oidcEnabled)
{
    if (webLoginEnabled)
    {
        app.UseCsrfProtection();
    }

    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapHealthChecks("/health");
app.MapMigrationCatalog();
app.MapCalendarFeeds();
app.MapBusinessEndpoints();

if (oidcEnabled)
{
    app.MapMemberEndpoints();
}

if (webLoginEnabled)
{
    app.MapAuthEndpoints();
}

app.Run();

static string? FindEnvFile()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory is not null)
    {
        var candidate = Path.Combine(directory.FullName, ".env");
        if (File.Exists(candidate))
        {
            return candidate;
        }

        directory = directory.Parent;
    }

    return null;
}
