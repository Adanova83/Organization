namespace Library.Contract;

public class ActivitySession : PrimaryKey
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float Price { get; set; } = 0.0f;
    public Guid ActivityId { get; set; }
    public Guid OrganizationSessionId { get; set; }
    public Active Active { get; set; } = default!;
}
