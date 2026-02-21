namespace Library.Domain;

public class EventAnimation : PrimaryKey
{
    public Guid EventId { get; set; }
    public Guid MemberId { get; set; }
    public int Order { get; set; }
}
