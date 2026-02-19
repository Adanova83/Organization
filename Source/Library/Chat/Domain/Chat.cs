namespace Library.Domain;

public class Chat : PrimaryKey
{
    public string? Title { get; set; }
    public Guid MemberId { get; set; }
    public Active Active { get; set; } = default!;
}
