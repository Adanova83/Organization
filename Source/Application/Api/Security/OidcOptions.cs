namespace Api.Security;

/// <summary>
/// Authentification OIDC avec l'annuaire du domaine (Authentik...).
/// - Jetons : l'API valide les jetons d'accès (JWT) présentés en Bearer. Sans Authority, tout est désactivé.
/// - Site web : si ClientId est renseigné, l'API mène elle-même la connexion (code + PKCE) et remet au navigateur
///   un cookie de session HttpOnly ; les jetons de l'annuaire ne quittent jamais le serveur.
/// </summary>
public sealed class OidcOptions
{
    public const string Section = "Oidc";

    /// <summary>URL de l'émetteur (ex. https://auth.adanova83.org/application/o/organizations/). Vide = désactivé.</summary>
    public string? Authority { get; set; }

    /// <summary>Audience attendue dans les jetons Bearer. Vide = l'audience n'est pas vérifiée.</summary>
    public string? Audience { get; set; }

    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>Identifiant client du fournisseur OIDC utilisé pour la connexion du site web. Vide = connexion web désactivée.</summary>
    public string? ClientId { get; set; }

    /// <summary>Secret du client (confidentiel : variable d'environnement, jamais dans le dépôt).</summary>
    public string? ClientSecret { get; set; }

    /// <summary>Scopes demandés à la connexion du site web.</summary>
    public string[] Scopes { get; set; } = ["openid", "profile", "email"];

    /// <summary>Claim contenant l'e-mail, utilisé pour rattacher le compte à un membre à la première connexion.</summary>
    public string EmailClaim { get; set; } = "email";

    /// <summary>
    /// Claim listant les groupes de l'annuaire de l'utilisateur (un groupe = un rôle).
    /// Avec Authentik : doit figurer dans le jeton d'identité (mapping de scope « groups » ou « profile »).
    /// </summary>
    public string GroupsClaim { get; set; } = "groups";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(Authority);

    public bool WebLoginEnabled => IsConfigured && !string.IsNullOrWhiteSpace(ClientId);
}
