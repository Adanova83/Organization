namespace Api.Accounts;

/// <summary>
/// Liaison avec l'API d'Authentik pour gérer les comptes des membres depuis l'intranet (section « Directory »).
/// Sans adresse ni jeton, la fonction est désactivée : les comptes se gèrent alors uniquement dans Authentik.
/// </summary>
public sealed class DirectoryOptions
{
    public const string Section = "Directory";

    /// <summary>Adresse d'Authentik, par exemple https://auth.adanova83.org.</summary>
    public string? BaseUrl { get; set; }

    /// <summary>Jeton d'API d'un compte de service aux droits limités (variable d'environnement, jamais dans le dépôt).</summary>
    public string? Token { get; set; }

    /// <summary>Groupe donné à tout compte créé ou rattaché par l'intranet.</summary>
    public string MemberGroup { get; set; } = "membres";

    /// <summary>Groupe des animateurs, que les responsables donnent ou retirent depuis la fiche du membre.</summary>
    public string AnimatorGroup { get; set; } = "animateurs";

    /// <summary>Dossier (chemin) des comptes créés par l'intranet dans l'annuaire d'Authentik.</summary>
    public string UserPath { get; set; } = "intranet";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(BaseUrl) && !string.IsNullOrWhiteSpace(Token);
}
