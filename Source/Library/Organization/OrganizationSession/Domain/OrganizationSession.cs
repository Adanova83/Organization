namespace Library.Domain;

public class OrganizationSession : PrimaryKey
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float Price { get; set; } = 0.0f;
    public Guid OrganizationId { get; set; }
}
