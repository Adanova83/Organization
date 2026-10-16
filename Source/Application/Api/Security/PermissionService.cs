using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace Api.Security;

/// <summary>Déduit rôles et permissions de l'utilisateur à partir des groupes présents dans son jeton.</summary>
public sealed class PermissionService
{
    public const string Wildcard = "*";

    private readonly OidcOptions _oidc;
    private readonly string? _adminGroup;
    private readonly Dictionary<string, string[]> _rolePermissions;

    public PermissionService(IOptions<OidcOptions> oidc, IOptions<PermissionOptions> permissions, IOptions<AdminOptions> admin)
    {
        _oidc = oidc.Value;
        _adminGroup = admin.Value.Group;
        _rolePermissions = new Dictionary<string, string[]>(permissions.Value.RolePermissions, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Groupes (rôles) de l'utilisateur, sans doublon.</summary>
    public IReadOnlyList<string> GetRoles(ClaimsPrincipal user)
    {
        return user.FindAll(_oidc.GroupsClaim)
            .Select(claim => claim.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>Permissions de l'utilisateur : celles de ses rôles (configuration) et ses groupes nommés comme une permission.</summary>
    public IReadOnlyList<string> GetPermissions(ClaimsPrincipal user)
    {
        var roles = GetRoles(user);

        // Le groupe administrateur de l'annuaire a toutes les permissions.
        if (!string.IsNullOrWhiteSpace(_adminGroup) && roles.Contains(_adminGroup, StringComparer.OrdinalIgnoreCase))
        {
            return new[] { Wildcard };
        }

        // Un groupe portant le nom d'une permission la donne directement ; les autres passent par la configuration.
        var direct = roles.Where(role => Permissions.All.Contains(role, StringComparer.OrdinalIgnoreCase));

        return roles
            .Where(role => _rolePermissions.ContainsKey(role))
            .SelectMany(role => _rolePermissions[role])
            .Concat(direct)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(permission => permission, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public bool HasPermission(ClaimsPrincipal user, string permission)
    {
        var granted = GetPermissions(user);

        if (granted.Contains(Wildcard) || granted.Contains(permission, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }

        // L'écriture d'un domaine donne aussi sa lecture.
        var implying = Permissions.EditImplying(permission);
        return implying is not null && granted.Contains(implying, StringComparer.OrdinalIgnoreCase);
    }
}
