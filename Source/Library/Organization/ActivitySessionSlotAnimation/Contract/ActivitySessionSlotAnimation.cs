namespace Library.Contract;

public class ActivitySessionSlotAnimation : PrimaryKey
{
    public Guid SessionSlotId { get; set; }
    public Guid MemberId{ get; set; }
    public int Order { get; set; }
    public Active Active { get; set; } = default!;
}
