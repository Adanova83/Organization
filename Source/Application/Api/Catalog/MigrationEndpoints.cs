using Api.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Catalog;

public static class MigrationEndpoints
{
    public static IEndpointRouteBuilder MapMigrationCatalog(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api")
            .AddEndpointFilter<ApiKeyFilter>();

        var migrations = api.MapGroup("/migrations").WithTags("Migrations");

        // Liste de toutes les migrations avec leur état (appliquée ou non).
        migrations.MapGet("/", async (MigrationCatalog catalog, CancellationToken ct)
            => Results.Ok(await catalog.ListAsync(ct)));

        // Résumé : combien de migrations appliquées / en attente, et écart modèle / snapshot.
        migrations.MapGet("/status", async (MigrationCatalog catalog, CancellationToken ct)
            => Results.Ok(await catalog.GetStatusAsync(ct)));

        // Script SQL entre deux migrations (from absent = base vide, to absent = dernière migration).
        migrations.MapGet("/script", (
            MigrationCatalog catalog,
            [FromQuery(Name = "from")] string? fromMigration,
            [FromQuery(Name = "to")] string? toMigration,
            bool? idempotent,
            bool? noTransactions) =>
        {
            if (!catalog.IsKnownTarget(fromMigration))
            {
                return Results.NotFound(new { message = $"Unknown migration '{fromMigration}'." });
            }

            if (!catalog.IsKnownTarget(toMigration))
            {
                return Results.NotFound(new { message = $"Unknown migration '{toMigration}'." });
            }

            var options = MigrationsSqlGenerationOptions.Default;
            if (idempotent == true)
            {
                options |= MigrationsSqlGenerationOptions.Idempotent;
            }

            if (noTransactions == true)
            {
                options |= MigrationsSqlGenerationOptions.NoTransactions;
            }

            return Results.Text(catalog.GenerateScript(fromMigration, toMigration, options), "text/plain");
        });

        // Détail d'une migration : opérations et SQL up / down.
        migrations.MapGet("/{id}", async (string id, MigrationCatalog catalog, CancellationToken ct) =>
        {
            var detail = await catalog.GetAsync(id, ct);
            return detail is null
                ? Results.NotFound(new { message = $"Unknown migration '{id}'." })
                : Results.Ok(detail);
        });

        // Applique (ou annule) des migrations. dryRun=true par défaut : renvoie seulement le script.
        migrations.MapPost("/apply", async (
            MigrationCatalog catalog,
            string? target,
            bool? dryRun,
            bool? allowRollback,
            CancellationToken ct) =>
        {
            var result = await catalog.ApplyAsync(target, dryRun ?? true, allowRollback ?? false, ct);

            return result.Status switch
            {
                ApplyStatus.UnknownTarget => Results.NotFound(result),
                ApplyStatus.RollbackNotAllowed or ApplyStatus.ModelHasPendingChanges => Results.Conflict(result),
                _ => Results.Ok(result)
            };
        });

        // Description du modèle courant de Library (tables, colonnes, clés, index).
        var model = api.MapGroup("/model").WithTags("Model");

        model.MapGet("/", (MigrationCatalog catalog) => Results.Ok(catalog.GetModel()));

        model.MapGet("/tables/{schema}/{name}", (string schema, string name, MigrationCatalog catalog) =>
        {
            var table = catalog.GetModel().FirstOrDefault(t =>
                string.Equals(t.Schema, schema, StringComparison.OrdinalIgnoreCase)
                && string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));

            return table is null
                ? Results.NotFound(new { message = $"Unknown table '{schema}.{name}'." })
                : Results.Ok(table);
        });

        return app;
    }
}
