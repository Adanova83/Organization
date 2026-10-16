using Ct = Library.Contract;

namespace Web.Clients;

/// <summary>Réponse de GET /api/me : membre connecté, groupes, rôles (présentation) et permissions.</summary>
public sealed class MeResponse
{
    public Ct.Member Member { get; set; } = default!;
    public List<string> Groups { get; set; } = [];
    public List<Ct.Role> Roles { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
}

public enum MeStatus
{
    /// <summary>Session valide et membre trouvé.</summary>
    Ok,
    /// <summary>Pas de session, ou session expirée.</summary>
    Unauthorized,
    /// <summary>Session valide mais aucun membre lié au compte.</summary>
    NoMember,
    /// <summary>API injoignable ou en erreur.</summary>
    Error
}

public sealed record MeResult(MeStatus Status, MeResponse? Me = null);
