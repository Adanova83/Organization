namespace Api.Security;

/// <summary>Session du site web : cookie émis par l'API après la connexion OIDC (schéma BFF).</summary>
public sealed class WebSessionOptions
{
    public const string Section = "Session";

    public string CookieName { get; set; } = "adanova.session";

    /// <summary>Durée de la session, en heures. Les groupes ne sont relus qu'à la reconnexion.</summary>
    public int LifetimeHours { get; set; } = 8;

    /// <summary>Adresse (relative) où revenir après connexion ou déconnexion quand aucune n'est demandée.</summary>
    public string DefaultReturnUrl { get; set; } = "/";

    /// <summary>true : la déconnexion ferme aussi la session de l'annuaire (SSO). false : seule la session du site est fermée.</summary>
    public bool EndProviderSessionOnLogout { get; set; }
}
