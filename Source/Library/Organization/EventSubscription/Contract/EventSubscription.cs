namespace Library.Contract;

public class EventSubscription : PrimaryKey
{
    public Guid EventId { get; set; }
    public Guid MemberId{ get; set; }
}
