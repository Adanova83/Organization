using System.Security.Claims;

namespace Web.Security;

/// <summary>
/// Permissions de l'application (mêmes noms que l'API, qui reste seule juge : le site s'en sert pour masquer
/// ce que l'utilisateur ne peut pas faire). *.edit donne aussi *.read ; « * » donne tout.
/// </summary>
public static class Permissions
{
    public const string ClaimType = "permission";
    public const string Wildcard = "*";

    public const string MemberRead = "member.read";
    public const string MemberEdit = "member.edit";
    public const string SpaceRead = "space.read";
    public const string SpaceEdit = "space.edit";
    public const string ActivityRead = "activity.read";
    public const string ActivityEdit = "activity.edit";
    public const string OrganizationRead = "organization.read";
    public const string OrganizationEdit = "organization.edit";
    public const string EventRead = "event.read";
    public const string EventEdit = "event.edit";
    public const string ProjectRead = "project.read";
    public const string ProjectEdit = "project.edit";
    public const string SubscriptionRead = "subscription.read";
    public const string SubscriptionEdit = "subscription.edit";
    public const string ContributionRead = "contribution.read";
    public const string ContributionEdit = "contribution.edit";
    public const string AccessRead = "access.read";
    public const string AccessEdit = "access.edit";

    public static readonly IReadOnlyList<string> All =
    [
        MemberRead, MemberEdit, SpaceRead, SpaceEdit, ActivityRead, ActivityEdit,
        OrganizationRead, OrganizationEdit, EventRead, EventEdit, ProjectRead, ProjectEdit,
        SubscriptionRead, SubscriptionEdit, ContributionRead, ContributionEdit, AccessRead, AccessEdit
    ];

    public static bool Has(ClaimsPrincipal user, string permission)
    {
        var granted = user.FindAll(ClaimType).Select(claim => claim.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (granted.Contains(Wildcard) || granted.Contains(permission))
        {
            return true;
        }

        // L'écriture d'un domaine donne aussi sa lecture (activity.read <- activity.edit).
        return permission.EndsWith(".read", StringComparison.OrdinalIgnoreCase)
            && granted.Contains(permission[..^".read".Length] + ".edit");
    }
}
