namespace Library.Contract;

public class EventAnimation : PrimaryKey
{
    public Guid EventId { get; set; }
    public Guid MemberId { get; set; }
    public int Order { get; set; }
    public Active Active { get; set; } = default!;
}
