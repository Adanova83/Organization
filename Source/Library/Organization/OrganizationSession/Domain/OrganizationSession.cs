namespace Library.Domain;

public class OrganizationSession : PrimaryKey
{
    public Guid SchoolYearId { get; set; }
    public float Price { get; set; } = 0.0f;
    public Guid OrganizationId { get; set; }

    // Année scolaire (lecture seule, renseignée à la lecture).
    public string? SchoolYearCode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
