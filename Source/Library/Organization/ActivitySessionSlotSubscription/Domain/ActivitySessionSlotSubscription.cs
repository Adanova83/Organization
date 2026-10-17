namespace Library.Domain;

public class ActivitySessionSlotSubscription : PrimaryKey
{
    public Guid SessionSlotId { get; set; }
    public Guid MemberId{ get; set; }
    public bool IsWaiting { get; set; }
    public int? WaitingRank { get; set; }
}
