namespace Library.Domain;

public class ProjectSlotSubscription : PrimaryKey
{
    public Guid ProjectSlotId { get; set; }
    public Guid MemberId{ get; set; }
    public bool IsWaiting { get; set; }
    public int? WaitingRank { get; set; }
}
