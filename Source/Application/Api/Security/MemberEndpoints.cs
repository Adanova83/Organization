using Library.Mapper;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using H = Library.Handler;
using Ct = Library.Contract;
using Do = Library.Domain;

namespace Api.Security;

/// <summary>Champs du profil qu'un membre peut modifier lui-même (ni e-mail, ni lien avec l'annuaire, ni période de validité).</summary>
public sealed class ProfileUpdate
{
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = default!;
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public DateTime? Birthdate { get; set; }
    public string? PhoneNumber { get; set; }
    public Ct.Address Addr { get; set; } = default!;
}

public static class MemberEndpoints
{
    /// <summary>Endpoints réservés aux utilisateurs authentifiés par jeton OIDC.</summary>
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api")
            .RequireAuthorization()
            .WithTags("Me");

        // Membre correspondant au compte connecté, avec ses rôles et permissions.
        // Première connexion : le compte est rattaché au membre portant le même e-mail (si l'e-mail est vérifié).
        api.MapGet("/me", async (
            ClaimsPrincipal user,
            CurrentMember currentMember,
            H.Role roleDetails,
            PermissionService permissions,
            CancellationToken ct) =>
        {
            var (member, error) = await currentMember.FindAsync(user, ct);
            if (member is null)
            {
                return error!;
            }

            // Rôles = groupes de l'annuaire présents dans le jeton ; la table Role n'apporte que la présentation (couleur...).
            var groups = permissions.GetRoles(user);
            var roles = new List<Ct.Role>();
            foreach (var group in groups)
            {
                var detail = await roleDetails.SearchByCodeAsync(group, ct);
                if (detail.Data is not null)
                {
                    roles.Add(detail.Data.DomainToContract());
                }
            }

            return Results.Ok(new
            {
                Member = member.DomainToContract(),
                Groups = groups,
                Roles = roles,
                Permissions = permissions.GetPermissions(user)
            });
        });

        // Modification de son propre profil, sans permission particulière.
        api.MapPut("/me", async (
            ProfileUpdate body,
            ClaimsPrincipal user,
            CurrentMember currentMember,
            H.Member members,
            CancellationToken ct) =>
        {
            var (member, error) = await currentMember.FindAsync(user, ct);
            if (member is null)
            {
                return error!;
            }

            member.Firstname = body.Firstname;
            member.Lastname = body.Lastname;
            member.Picture = body.Picture;
            member.RGBColor = body.RGBColor;
            member.Birthdate = body.Birthdate;
            member.PhoneNumber = body.PhoneNumber;
            member.Addr = body.Addr.ContractToDomain();

            await members.EditAsync(member, ct);

            var updated = await members.SearchAsync(member.Id, ct);
            return updated.Data is null ? Results.NotFound() : Results.Ok(updated.Data.DomainToContract());
        });

        return app;
    }
}
