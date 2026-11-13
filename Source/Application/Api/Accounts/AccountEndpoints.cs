using Api.Security;
using Microsoft.AspNetCore.Mvc;
using Ct = Library.Contract;
using Do = Library.Domain;
using H = Library.Handler;

namespace Api.Accounts;

/// <summary>
/// Compte de connexion d'un membre, géré depuis sa fiche : /api/members/{id}/account.
/// Lecture : member.read ; création, modification, nouveau mot de passe : member.edit.
/// </summary>
public static class AccountEndpoints
{
    public static IServiceCollection AddMemberAccounts(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DirectoryOptions>(configuration.GetSection(DirectoryOptions.Section));
        services.AddHttpClient<IDirectoryClient, AuthentikClient>(client => client.Timeout = TimeSpan.FromSeconds(20));
        services.AddScoped<AccountService>();
        return services;
    }

    public static IEndpointRouteBuilder MapMemberAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/members/{id:guid}/account").WithTags("Members");

        group.MapGet("/", (Guid id, [FromServices] H.Member members, [FromServices] AccountService accounts, CancellationToken ct)
                => RunAsync(id, members, member => accounts.GetAsync(member, ct), ct))
            .RequirePermission(Permissions.MemberRead);

        group.MapPost("/", (Guid id, Ct.MemberAccountCreate body, [FromServices] H.Member members, [FromServices] AccountService accounts, CancellationToken ct)
                => RunAsync(id, members, member => accounts.CreateAsync(member, body.IsAnimator, ct), ct))
            .RequirePermission(Permissions.MemberEdit);

        group.MapPut("/", (Guid id, Ct.MemberAccountUpdate body, [FromServices] H.Member members, [FromServices] AccountService accounts, CancellationToken ct)
                => RunAsync(id, members, member => accounts.UpdateAsync(member, body, ct), ct))
            .RequirePermission(Permissions.MemberEdit);

        group.MapPost("/reset-password", (Guid id, [FromServices] H.Member members, [FromServices] AccountService accounts, CancellationToken ct)
                => RunAsync(id, members, member => accounts.ResetPasswordAsync(member, ct), ct))
            .RequirePermission(Permissions.MemberEdit);

        return app;
    }

    private static async Task<IResult> RunAsync(Guid id, H.Member members, Func<Do.Member, Task<Ct.MemberAccount>> action, CancellationToken ct)
    {
        var member = (await members.SearchAsync(id, ct)).Data;
        if (member is null)
        {
            return Results.NotFound();
        }

        try
        {
            return Results.Ok(await action(member));
        }
        catch (AccountConflictException ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status409Conflict, title: ex.Message);
        }
        catch (DirectoryException ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status502BadGateway, title: ex.Message);
        }
    }
}
