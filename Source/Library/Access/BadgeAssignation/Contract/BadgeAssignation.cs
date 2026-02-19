namespace Library.Contract;

public class BadgeAssignation : PrimaryKey
{
    public Guid BadgeId { get; set; } = Guid.Empty;
    public Guid MemberId { get; set; } = Guid.Empty;
    public Active Active { get; set; } = null!;
}
