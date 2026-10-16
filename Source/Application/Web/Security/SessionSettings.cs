namespace Web.Security;

/// <summary>Nom du cookie de session émis par l'API (doit correspondre à Session:CookieName de l'API).</summary>
public sealed class SessionSettings
{
    public string CookieName { get; set; } = "adanova.session";
}
