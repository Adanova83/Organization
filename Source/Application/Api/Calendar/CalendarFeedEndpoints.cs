using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Api.Calendar;

public static class CalendarFeedEndpoints
{
    public static IServiceCollection AddCalendarFeeds(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CalendarFeedOptions>(configuration.GetSection(CalendarFeedOptions.Section));
        services.AddScoped<CalendarFeedService>();
        return services;
    }

    /// <summary>
    /// GET /api/calendar/{events|activities|projects|holidays|all}.ics?token=...
    /// À utiliser comme adresse d'abonnement dans l'application Agenda de Nextcloud.
    /// </summary>
    public static IEndpointRouteBuilder MapCalendarFeeds(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/calendar/{feed}.ics", async (
            string feed,
            string? token,
            IOptions<CalendarFeedOptions> options,
            CalendarFeedService service,
            HttpContext context,
            CancellationToken ct) =>
        {
            var configured = options.Value.FeedToken;

            if (string.IsNullOrWhiteSpace(configured))
            {
                return Results.NotFound();
            }

            if (!TokenMatches(configured, token))
            {
                return Results.Unauthorized();
            }

            var ics = await service.BuildAsync(feed, ct);
            if (ics is null)
            {
                return Results.NotFound();
            }

            context.Response.Headers.CacheControl = "private, max-age=300";
            return Results.Text(ics, "text/calendar", Encoding.UTF8);
        })
        .WithTags("Calendar");

        return app;
    }

    private static bool TokenMatches(string expected, string? provided)
    {
        if (string.IsNullOrEmpty(provided))
        {
            return false;
        }

        // Comparaison en temps constant, sur des empreintes de même longueur.
        var left = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
        var right = SHA256.HashData(Encoding.UTF8.GetBytes(provided));
        return CryptographicOperations.FixedTimeEquals(left, right);
    }
}
