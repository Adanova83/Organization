namespace Library.Contract;

public class ProjectSlotSubscription : PrimaryKey
{
    public Guid ProjectSlotId { get; set; }
    public Guid MemberId{ get; set; }
    public Active Active { get; set; } = default!;
}
