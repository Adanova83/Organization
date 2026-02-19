namespace Library.Domain;

public class ActivitySessionSlotSubscription : PrimaryKey
{
    public Guid SessionSlotId { get; set; }
    public Guid MemberId{ get; set; }
    public Active Active { get; set; } = default!;
}
