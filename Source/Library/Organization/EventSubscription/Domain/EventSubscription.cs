namespace Library.Domain;

public class EventSubscription : PrimaryKey
{
    public Guid EventId { get; set; }
    public Guid MemberId{ get; set; }
    public bool IsWaiting { get; set; }
    public int? WaitingRank { get; set; }
}
