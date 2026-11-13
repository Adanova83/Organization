namespace Library.Contract;

/// <summary>Compte de connexion (Authentik) d'un membre, piloté depuis sa fiche.</summary>
public sealed class MemberAccount
{
    /// <summary>Faux si l'intranet n'est pas relié à Authentik (configuration absente) : rien n'est géré depuis l'intranet.</summary>
    public bool Enabled { get; set; }

    /// <summary>Le membre a un compte (créé ou rattaché par l'intranet).</summary>
    public bool Exists { get; set; }

    public string? Username { get; set; }

    public bool IsActive { get; set; }

    /// <summary>Le compte est dans le groupe des animateurs.</summary>
    public bool IsAnimator { get; set; }

    /// <summary>Compte administrateur d'Authentik : l'intranet ne le modifie jamais (nom, e-mail, mot de passe, groupes).</summary>
    public bool IsProtected { get; set; }

    /// <summary>Mot de passe provisoire, renvoyé une seule fois (création ou réinitialisation), jamais stocké par l'intranet.</summary>
    public string? TemporaryPassword { get; set; }
}

/// <summary>Création du compte d'un membre.</summary>
public sealed class MemberAccountCreate
{
    public bool IsAnimator { get; set; }
}

/// <summary>Modification du compte : groupe des animateurs, compte actif ou désactivé.</summary>
public sealed class MemberAccountUpdate
{
    public bool IsAnimator { get; set; }

    public bool IsActive { get; set; }
}
