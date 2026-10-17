namespace Library.Contract;

public class Contribution : PrimaryKey
{
    public Guid MemberId { get; set; }
    public Guid OrganizationSessionId { get; set; }
    public DateOnly PayedDate { get; set; }
    /// <summary>Montant payé (euros) ; plusieurs paiements possibles pour une même saison.</summary>
    public float Amount { get; set; }
}
