namespace Library.Contract;

public class Activity : PrimaryKey
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public int AgeMinimum { get; set; } = 0;
    public Guid OrganizationId { get; set; }
    public Active Active { get; set; } = default!;
}
