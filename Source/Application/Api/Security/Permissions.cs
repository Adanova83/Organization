namespace Api.Security;

/// <summary>
/// Permissions de l'application. Une permission s'obtient de deux façons : par la correspondance
/// groupe -> permissions (Authorization:RolePermissions), ou en appartenant à un groupe de l'annuaire
/// portant exactement le nom de la permission (par exemple le groupe « activity.edit »).
/// </summary>
public static class Permissions
{
    // Chaque domaine a une permission d'écriture (*.edit) et une de lecture (*.read) ;
    // *.edit donne aussi *.read, inutile d'attribuer les deux.

    /// <summary>Membres : lecture / écriture (données personnelles comprises).</summary>
    public const string MemberRead = "member.read";
    public const string MemberEdit = "member.edit";

    /// <summary>Espaces.</summary>
    public const string SpaceRead = "space.read";
    public const string SpaceEdit = "space.edit";

    /// <summary>Ateliers : activités, sessions et créneaux, et leurs animateurs.</summary>
    public const string ActivityRead = "activity.read";
    public const string ActivityEdit = "activity.edit";

    /// <summary>Organisations, saisons, jours fériés et pays.</summary>
    public const string OrganizationRead = "organization.read";
    public const string OrganizationEdit = "organization.edit";

    /// <summary>Événements et leurs animateurs.</summary>
    public const string EventRead = "event.read";
    public const string EventEdit = "event.edit";

    /// <summary>Projets, leurs créneaux et leurs animateurs.</summary>
    public const string ProjectRead = "project.read";
    public const string ProjectEdit = "project.edit";

    /// <summary>Inscriptions de tous les membres (un membre gère déjà les siennes).</summary>
    public const string SubscriptionRead = "subscription.read";
    public const string SubscriptionEdit = "subscription.edit";

    /// <summary>Cotisations de tous les membres (un membre lit déjà les siennes).</summary>
    public const string ContributionRead = "contribution.read";
    public const string ContributionEdit = "contribution.edit";

    /// <summary>Badges, attributions et passages.</summary>
    public const string AccessRead = "access.read";
    public const string AccessEdit = "access.edit";

    public static readonly IReadOnlyList<string> All = new[]
    {
        MemberRead, MemberEdit, SpaceRead, SpaceEdit, ActivityRead, ActivityEdit,
        OrganizationRead, OrganizationEdit, EventRead, EventEdit, ProjectRead, ProjectEdit,
        SubscriptionRead, SubscriptionEdit, ContributionRead, ContributionEdit, AccessRead, AccessEdit
    };

    private const string EditSuffix = ".edit";
    private const string ReadSuffix = ".read";

    /// <summary>Permission de lecture d'un domaine à partir de sa permission d'écriture (activity.edit -> activity.read).</summary>
    public static string ReadOf(string editPermission)
        => editPermission.EndsWith(EditSuffix, StringComparison.OrdinalIgnoreCase)
            ? editPermission[..^EditSuffix.Length] + ReadSuffix
            : editPermission;

    /// <summary>Permission d'écriture qui implique une permission de lecture, ou null (activity.read -> activity.edit).</summary>
    public static string? EditImplying(string permission)
        => permission.EndsWith(ReadSuffix, StringComparison.OrdinalIgnoreCase)
            ? permission[..^ReadSuffix.Length] + EditSuffix
            : null;
}
