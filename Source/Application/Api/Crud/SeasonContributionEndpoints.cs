using Api.Security;
using Microsoft.AspNetCore.Mvc;
using H = Library.Handler;

namespace Api.Crud;

/// <summary>
/// Cotisations d'une saison : pour chaque membre, montant dû, payé, reste, et s'il participe à une activité de
/// l'organisation cette année-là (« qui n'a pas payé cette saison »). Noms des membres inclus : contribution.read.
/// GET /api/contributions/seasons/{organizationSessionId}
/// </summary>
public static class SeasonContributionEndpoints
{
    public static IEndpointRouteBuilder MapSeasonContributions(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/contributions/seasons/{organizationSessionId:guid}", async (
                Guid organizationSessionId, [FromServices] H.Contribution handler, CancellationToken ct) =>
            {
                var season = await handler.SeasonAsync(organizationSessionId, ct);
                return season is null ? Results.NotFound() : Results.Ok(season);
            })
            .WithTags("Contributions")
            .RequirePermission(Permissions.ContributionRead);

        return app;
    }
}
