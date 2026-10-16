namespace Library.Domain;

public class Project : PrimaryKey
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public int AgeMinimum { get; set; } = 0;
    // Facultatives : par défaut, le projet couvre toute l'année scolaire.
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid SchoolYearId { get; set; }

    // Lecture seule (renseignés à la lecture) : année scolaire et dates effectives (propres, sinon celles de l'année).
    public string? SchoolYearCode { get; set; }
    public DateTime? EffectiveStartDate { get; set; }
    public DateTime? EffectiveEndDate { get; set; }
}
