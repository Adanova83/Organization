namespace Library.Domain;

public class Event : PrimaryKey
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? AgeMinimum { get; set; } = null;
    public Guid OrganizationSessionId { get; set; }
    public Guid? SpaceId { get; set; }
    public Active Active { get; set; } = default!;
}
