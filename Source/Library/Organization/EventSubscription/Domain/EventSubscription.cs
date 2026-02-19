namespace Library.Domain;

public class EventSubscription : PrimaryKey
{
    public Guid EventId { get; set; }
    public Guid MemberId{ get; set; }
    public Active Active { get; set; } = default!;
}
