using Library.Handler;
using Library.Repository;
using Microsoft.AspNetCore.Mvc;
using Api.Security;
using Ct = Library.Contract;
using Do = Library.Domain;

namespace Api.Crud;

/// <summary>Réponse paginée : éléments de la page, nombre après filtre et nombre total.</summary>
public sealed record PagedResponse<T>(IReadOnlyList<T> Data, int FilteredCount, int TotalCount);

public static class CrudEndpoints
{
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 200;

    /// <summary>
    /// Déclare GET (un élément, et liste si <paramref name="list"/> est fourni), POST, PUT et DELETE.
    /// Lecture : permission de lecture du domaine (<paramref name="readPermission"/>, par défaut déduite de <paramref name="writePermission"/>,
    /// par exemple activity.edit -> activity.read) ou clé d'API. Écriture : permission <paramref name="writePermission"/> ou clé d'API.
    /// <paramref name="readOnly"/> : seules les lectures sont déclarées.
    /// <paramref name="keepOnUpdate"/> : (nouveau, existant) recopie les champs non modifiables par le client.
    /// <paramref name="afterCreate"/> : traitement complémentaire après une création (import automatique...).
    /// <paramref name="afterChange"/> : traitement complémentaire après une modification ou une suppression (liste d'attente...).
    /// Retourne le groupe de routes pour y ajouter des routes enfants.
    /// </summary>
    public static RouteGroupBuilder MapCrud<TContract, TDomain, THandler>(
        this IEndpointRouteBuilder app,
        string route,
        string tag,
        string writePermission,
        Func<TContract, Guid, TDomain> toDomain,
        Func<TDomain, TContract> toContract,
        Func<THandler, string?, int, int, CancellationToken, Task<RepositoryResponseRange<TDomain>>>? list = null,
        string? readPermission = null,
        Action<TDomain, TDomain>? keepOnUpdate = null,
        bool readOnly = false,
        Func<TDomain, IServiceProvider, CancellationToken, Task>? afterCreate = null,
        Func<TDomain, IServiceProvider, CancellationToken, Task>? afterChange = null)
        where TContract : Ct.PrimaryKey
        where TDomain : Do.PrimaryKey
        where THandler : class, ICrudHandler<TDomain>
    {
        var group = app.MapGroup(route).WithTags(tag);

        // Lecture : permission *.read du domaine (ou *.edit, qui l'implique), déduite de la permission d'écriture.
        group.RequirePermission(readPermission ?? Permissions.ReadOf(writePermission));

        if (list is not null)
        {
            group.MapGet("/", async (
                string? query, int? page, int? pageSize,
                [FromServices] THandler handler, CancellationToken ct) =>
            {
                var (p, s) = Paging(page, pageSize);
                var result = await list(handler, query, p, s, ct);
                return Results.Ok(ToPage(result, toContract));
            });
        }

        group.MapGet("/{id:guid}", async (Guid id, [FromServices] THandler handler, CancellationToken ct) =>
        {
            var found = await handler.SearchAsync(id, ct);
            return found.Data is null ? Results.NotFound() : Results.Ok(toContract(found.Data));
        });

        if (readOnly)
        {
            return group;
        }

        group.MapPost("/", async (TContract body, [FromServices] THandler handler, HttpContext http, CancellationToken ct) =>
        {
            var created = await handler.CreateAsync(toDomain(body, Guid.Empty), ct);
            if (afterCreate is not null)
            {
                await afterCreate(created.Data!, http.RequestServices, ct);
            }

            var contract = toContract(created.Data!);
            return Results.Created($"{route}/{contract.Id}", contract);
        }).RequirePermission(writePermission);

        group.MapPut("/{id:guid}", async (Guid id, TContract body, [FromServices] THandler handler, HttpContext http, CancellationToken ct) =>
        {
            var existing = await handler.SearchAsync(id, ct);
            if (existing.Data is null)
            {
                return Results.NotFound();
            }

            var domain = toDomain(body, id);

            // Période de validité omise : on conserve celle de l'élément existant.
            if (body.Active is null)
            {
                domain.Active = existing.Data.Active;
            }

            // Champs que le client ne peut pas modifier : recopiés depuis l'élément existant.
            keepOnUpdate?.Invoke(domain, existing.Data);

            await handler.EditAsync(domain, ct);
            if (afterChange is not null)
            {
                await afterChange(domain, http.RequestServices, ct);
            }

            var updated = await handler.SearchAsync(id, ct);
            return updated.Data is null ? Results.NotFound() : Results.Ok(toContract(updated.Data));
        }).RequirePermission(writePermission);

        group.MapDelete("/{id:guid}", async (Guid id, [FromServices] THandler handler, HttpContext http, CancellationToken ct) =>
        {
            var existing = await handler.SearchAsync(id, ct);
            if (existing.Data is null)
            {
                return Results.NotFound();
            }

            // Suppression logique (voir FullDbContext) : la ligne est conservée avec Audit.DeletedAt.
            await handler.DeleteAsync(existing.Data, ct);
            if (afterChange is not null)
            {
                await afterChange(existing.Data, http.RequestServices, ct);
            }

            return Results.NoContent();
        }).RequirePermission(writePermission);

        return group;
    }

    /// <summary>Ajoute GET {route}/{parentId}/{childPath} : liste paginée des éléments rattachés à un parent.
    /// <paramref name="permission"/> : permission supplémentaire exigée par cette seule route (en plus de celle du groupe).</summary>
    public static RouteGroupBuilder MapChildren<TContract, TDomain, THandler>(
        this RouteGroupBuilder parent,
        string childPath,
        Func<THandler, Guid, int, int, CancellationToken, Task<RepositoryResponseRange<TDomain>>> list,
        Func<TDomain, TContract> toContract,
        string? permission = null)
        where THandler : class
    {
        var endpoint = parent.MapGet("/{parentId:guid}/" + childPath, async (
            Guid parentId, int? page, int? pageSize,
            [FromServices] THandler handler, CancellationToken ct) =>
        {
            var (p, s) = Paging(page, pageSize);
            var result = await list(handler, parentId, p, s, ct);
            return Results.Ok(ToPage(result, toContract));
        });

        if (permission is not null)
        {
            endpoint.RequirePermission(permission);
        }

        return parent;
    }

    /// <summary>Ajoute GET {route}?targetId=&memberId= : liste paginée filtrable par élément cible et par membre.</summary>
    public static RouteGroupBuilder MapFilteredList<TContract, TDomain, THandler>(
        this RouteGroupBuilder group,
        Func<THandler, Guid?, Guid?, int, int, CancellationToken, Task<RepositoryResponseRange<TDomain>>> list,
        Func<TDomain, TContract> toContract)
        where THandler : class
    {
        group.MapGet("/", async (
            Guid? targetId, Guid? memberId, int? page, int? pageSize,
            [FromServices] THandler handler, CancellationToken ct) =>
        {
            var (p, s) = Paging(page, pageSize);
            var result = await list(handler, targetId, memberId, p, s, ct);
            return Results.Ok(ToPage(result, toContract));
        });

        return group;
    }

    internal static (int Page, int PageSize) Paging(int? page, int? pageSize)
        => (Math.Max(page ?? 1, 1), Math.Clamp(pageSize ?? DefaultPageSize, 1, MaxPageSize));

    internal static PagedResponse<TContract> ToPage<TDomain, TContract>(
        RepositoryResponseRange<TDomain> result, Func<TDomain, TContract> toContract)
        => new(result.Data.Select(toContract).ToList(), result.FilteredCount, result.TotalCount);
}
