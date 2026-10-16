namespace Library.Domain;

public class ActivitySession : PrimaryKey
{
    public Guid ActivityId { get; set; }
    public Guid SchoolYearId { get; set; }
    public float Price { get; set; } = 0.0f;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool DuringTermTime { get; set; } = true;
    public bool DuringSchoolHolidays { get; set; }

    // Année scolaire et dates effectives (lecture seule, renseignées à la lecture).
    public string? SchoolYearCode { get; set; }
    public DateTime? EffectiveStartDate { get; set; }
    public DateTime? EffectiveEndDate { get; set; }
}
