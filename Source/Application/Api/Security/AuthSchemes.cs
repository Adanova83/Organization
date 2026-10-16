namespace Api.Security;

/// <summary>Noms des schémas d'authentification de l'API.</summary>
public static class AuthSchemes
{
    /// <summary>Schéma par défaut : jeton Bearer si l'en-tête Authorization en porte un, sinon cookie de session.</summary>
    public const string Smart = "Smart";

    /// <summary>Cookie de session du site web (schéma BFF).</summary>
    public const string Cookie = "Session";

    /// <summary>Connexion OIDC (code + PKCE) menée par l'API avec l'annuaire.</summary>
    public const string Oidc = "Oidc";
}
