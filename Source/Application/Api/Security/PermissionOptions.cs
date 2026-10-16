namespace Api.Security;

/// <summary>
/// Permissions accordées à chaque rôle. Les rôles sont les groupes de l'annuaire (Authentik/LDAP) ;
/// seule la correspondance groupe -> permissions est définie ici, dans la section « Authorization ».
/// Le caractère « * » donne toutes les permissions.
/// </summary>
public sealed class PermissionOptions
{
    public const string Section = "Authorization";

    /// <summary>Nom du groupe -> liste de permissions. Exemple : "animateurs": ["activity.edit"].</summary>
    public Dictionary<string, string[]> RolePermissions { get; set; } = new();
}
