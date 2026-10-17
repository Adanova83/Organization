namespace Library.Contract;

/// <summary>Situation d'un membre pour la cotisation d'une saison.</summary>
public enum ContributionState
{
    /// <summary>Rien payé.</summary>
    Unpaid,
    /// <summary>Payé en partie.</summary>
    Partial,
    /// <summary>À jour (cotisation payée, ou saison sans cotisation).</summary>
    Paid
}

/// <summary>Cotisation d'une saison pour un membre : montant dû, payé, reste à payer.</summary>
public sealed class MemberContribution
{
    public Guid MemberId { get; set; }
    public string Lastname { get; set; } = "";
    public string? Firstname { get; set; }
    /// <summary>Inscrit (ou en attente) à un atelier, un événement ou un projet de l'organisation cette année-là.</summary>
    public bool Participates { get; set; }
    public float Due { get; set; }
    public float Paid { get; set; }
    public float Remaining => Math.Max(Due - Paid, 0);
    public DateOnly? LastPayedDate { get; set; }
    public ContributionState State => Paid <= 0 && Due > 0 ? ContributionState.Unpaid
        : Paid < Due ? ContributionState.Partial
        : ContributionState.Paid;
}

/// <summary>
/// Cotisations d'une saison (organisation + année scolaire) : tous les membres actuels, avec ce qu'ils ont payé,
/// et les totaux sur les participants (membres inscrits à une activité de l'organisation cette année-là).
/// </summary>
public sealed class SeasonContributions
{
    public Guid OrganizationSessionId { get; set; }
    public Guid OrganizationId { get; set; }
    public string? SchoolYearCode { get; set; }
    public float Price { get; set; }
    public List<MemberContribution> Members { get; set; } = [];

    public int Participants => Members.Count(m => m.Participates);
    public int ParticipantsUpToDate => Members.Count(m => m.Participates && m.State == ContributionState.Paid);
    public int ParticipantsNotUpToDate => Participants - ParticipantsUpToDate;
    /// <summary>Total encaissé pour la saison (participants ou non).</summary>
    public float Collected => Members.Sum(m => m.Paid);
    /// <summary>Total attendu des participants.</summary>
    public float Expected => Members.Where(m => m.Participates).Sum(m => m.Due);
}
