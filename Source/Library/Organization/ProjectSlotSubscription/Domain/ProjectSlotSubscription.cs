namespace Library.Domain;

public class ProjectSlotSubscription : PrimaryKey
{
    public Guid ProjectSlotId { get; set; }
    public Guid MemberId{ get; set; }
}
