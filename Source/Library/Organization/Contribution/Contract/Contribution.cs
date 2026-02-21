namespace Library.Contract;

public class Contribution : PrimaryKey
{
    public Guid MemberId { get; set; }
    public Guid OrganizationSessionId { get; set; }
    public DateOnly PayedDate { get; set; }
}
