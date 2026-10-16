namespace Library.Contract;

/// <summary>Saison d'une organisation : son rattachement à une année scolaire, avec la cotisation de l'année.</summary>
public class OrganizationSession : PrimaryKey
{
    public Guid SchoolYearId { get; set; }
    public float Price { get; set; } = 0.0f;
    public Guid OrganizationId { get; set; }

    // Renseignés par l'API depuis l'année scolaire (lecture seule, ignorés à l'envoi).
    public string? SchoolYearCode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
