using Api.Security;
using Library.Handler;
using Library.Repository;
using Microsoft.AspNetCore.Mvc;
using Ct = Library.Contract;
using Do = Library.Domain;

namespace Api.Crud;

/// <summary>
/// CRUD pour des données qui appartiennent à un membre (inscriptions, cotisations, notes...).
/// Accès complet : clé d'API (si autorisée) ou permission « staff » (*.read pour lire, *.edit pour écrire). Sinon, le membre connecté n'accède
/// qu'à ses propres éléments ; l'écriture n'est possible que si <c>ownerCanWrite</c> est vrai.
/// </summary>
public static class OwnedCrudEndpoints
{
    public static RouteGroupBuilder MapOwnedCrud<TContract, TDomain, THandler>(
        this IEndpointRouteBuilder app,
        string route,
        string tag,
        string? staffPermission,
        bool ownerCanWrite,
        Func<TContract, Guid, TDomain> toDomain,
        Func<TDomain, TContract> toContract,
        Func<TDomain, IServiceProvider, CancellationToken, Task<Guid?>> ownerOf,
        Func<THandler, Guid?, Guid?, int, int, CancellationToken, Task<RepositoryResponseRange<TDomain>>> list,
        Func<Guid, IServiceProvider, CancellationToken, Task<Guid?>>? ownerOfTarget = null,
        Func<TDomain, bool, IServiceProvider, CancellationToken, Task>? beforeWrite = null)
        where TContract : Ct.PrimaryKey
        where TDomain : Do.PrimaryKey
        where THandler : class, ICrudHandler<TDomain>
    {
        var group = app.MapGroup(route).WithTags(tag).RequireAccess();

        // targetId : élément auquel se rattachent les données (créneau, événement, note...) ; memberId : leur propriétaire.
        group.MapGet("/", async (
            Guid? targetId, Guid? memberId, int? page, int? pageSize,
            HttpContext http, [FromServices] THandler handler, CancellationToken ct) =>
        {
            var access = await ResolveAsync(http, staffPermission, write: false, ct);
            if (access.Error is not null)
            {
                return access.Error;
            }

            if (!access.Full)
            {
                if (ownerOfTarget is not null)
                {
                    // Liste rattachée à un élément d'autrui (ex. liens d'une note) : son propriétaire doit être l'appelant.
                    if (targetId is null)
                    {
                        return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "targetId is required.");
                    }

                    if (await ownerOfTarget(targetId.Value, http.RequestServices, ct) != access.Me!.Id)
                    {
                        return Results.NotFound();
                    }
                }
                else
                {
                    if (memberId is not null && memberId != access.Me!.Id)
                    {
                        return Forbidden();
                    }

                    memberId = access.Me!.Id;
                }
            }

            var (p, s) = CrudEndpoints.Paging(page, pageSize);
            var result = await list(handler, targetId, memberId, p, s, ct);
            return Results.Ok(CrudEndpoints.ToPage(result, toContract));
        });

        group.MapGet("/{id:guid}", async (
            Guid id, HttpContext http, [FromServices] THandler handler, CancellationToken ct) =>
        {
            var access = await ResolveAsync(http, staffPermission, write: false, ct);
            if (access.Error is not null)
            {
                return access.Error;
            }

            var found = await handler.SearchAsync(id, ct);
            if (found.Data is null || !await IsAllowedAsync(access, found.Data, ownerOf, http, ct))
            {
                return Results.NotFound();
            }

            return Results.Ok(toContract(found.Data));
        });

        group.MapPost("/", async (
            TContract body, HttpContext http, [FromServices] THandler handler, CancellationToken ct) =>
        {
            var access = await ResolveAsync(http, staffPermission, write: true, ct);
            if (access.Error is not null)
            {
                return access.Error;
            }

            var domain = toDomain(body, Guid.Empty);
            if (!access.Full && (!ownerCanWrite || !await IsAllowedAsync(access, domain, ownerOf, http, ct)))
            {
                return Forbidden();
            }

            // Règles métier (inscriptions : doublons, capacité, âge) ; accès complet = dérogation possible.
            if (beforeWrite is not null)
            {
                await beforeWrite(domain, access.Full, http.RequestServices, ct);
            }

            var created = await handler.CreateAsync(domain, ct);
            var contract = toContract(created.Data!);
            return Results.Created($"{route}/{contract.Id}", contract);
        });

        group.MapPut("/{id:guid}", async (
            Guid id, TContract body, HttpContext http, [FromServices] THandler handler, CancellationToken ct) =>
        {
            var access = await ResolveAsync(http, staffPermission, write: true, ct);
            if (access.Error is not null)
            {
                return access.Error;
            }

            var existing = await handler.SearchAsync(id, ct);
            if (existing.Data is null || !await IsAllowedAsync(access, existing.Data, ownerOf, http, ct))
            {
                return Results.NotFound();
            }

            var domain = toDomain(body, id);
            if (!access.Full && (!ownerCanWrite || !await IsAllowedAsync(access, domain, ownerOf, http, ct)))
            {
                return Forbidden();
            }

            // Période de validité omise : on conserve celle de l'élément existant.
            if (body.Active is null)
            {
                domain.Active = existing.Data.Active;
            }

            if (beforeWrite is not null)
            {
                await beforeWrite(domain, access.Full, http.RequestServices, ct);
            }

            await handler.EditAsync(domain, ct);

            var updated = await handler.SearchAsync(id, ct);
            return updated.Data is null ? Results.NotFound() : Results.Ok(toContract(updated.Data));
        });

        group.MapDelete("/{id:guid}", async (
            Guid id, HttpContext http, [FromServices] THandler handler, CancellationToken ct) =>
        {
            var access = await ResolveAsync(http, staffPermission, write: true, ct);
            if (access.Error is not null)
            {
                return access.Error;
            }

            var existing = await handler.SearchAsync(id, ct);
            if (existing.Data is null || !await IsAllowedAsync(access, existing.Data, ownerOf, http, ct))
            {
                return Results.NotFound();
            }

            if (!access.Full && !ownerCanWrite)
            {
                return Forbidden();
            }

            await handler.DeleteAsync(existing.Data, ct);
            return Results.NoContent();
        });

        return group;
    }

    /// <summary>Propriétaire = un champ Guid du domaine (MemberId).</summary>
    public static Func<TDomain, IServiceProvider, CancellationToken, Task<Guid?>> OwnerField<TDomain>(Func<TDomain, Guid> memberId)
        => (domain, _, _) => Task.FromResult<Guid?>(memberId(domain));

    private sealed record Access(bool Full, Do.Member? Me, IResult? Error);

    private static async Task<Access> ResolveAsync(HttpContext http, string? staffPermission, bool write, CancellationToken ct)
    {
        if (http.HasValidApiKey())
        {
            return new Access(true, null, null);
        }

        // Lecture de toutes les données : *.read (ou *.edit) ; écriture : *.edit.
        if (staffPermission is not null
            && http.User.Identity?.IsAuthenticated == true
            && http.RequestServices.GetRequiredService<PermissionService>()
                .HasPermission(http.User, write ? staffPermission : Permissions.ReadOf(staffPermission)))
        {
            return new Access(true, null, null);
        }

        var (me, error) = await http.RequestServices.GetRequiredService<CurrentMember>().FindAsync(http.User, ct);
        return me is null ? new Access(false, null, error) : new Access(false, me, null);
    }

    private static async Task<bool> IsAllowedAsync<TDomain>(
        Access access, TDomain domain,
        Func<TDomain, IServiceProvider, CancellationToken, Task<Guid?>> ownerOf,
        HttpContext http, CancellationToken ct)
    {
        return access.Full || await ownerOf(domain, http.RequestServices, ct) == access.Me!.Id;
    }

    private static IResult Forbidden() => Results.Problem(
        statusCode: StatusCodes.Status403Forbidden,
        title: "Forbidden.",
        detail: "You can only change your own data, or you lack the required permission.");
}
