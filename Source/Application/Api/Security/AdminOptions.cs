namespace Api.Security;

/// <summary>
/// Premier compte administrateur. Le mot de passe n'est jamais connu de cette API : le compte est créé dans
/// l'annuaire (Authentik : variables AUTHENTIK_BOOTSTRAP_EMAIL / AUTHENTIK_BOOTSTRAP_PASSWORD) et seul son
/// e-mail sert ici, à créer le membre correspondant au démarrage et à le rattacher à la première connexion.
/// </summary>
public sealed class AdminOptions
{
    public const string Section = "Admin";

    /// <summary>E-mail du compte administrateur. Vide = aucun membre créé automatiquement.</summary>
    public string? Email { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    /// <summary>
    /// Groupe de l'annuaire dont les membres ont toutes les permissions.
    /// « authentik Admins » est le groupe d'administration d'Authentik, auquel appartient le compte initial.
    /// Vide = aucun groupe administrateur.
    /// </summary>
    public string? Group { get; set; } = "authentik Admins";
}
