namespace Library.Domain;

public class ChatMessage : PrimaryKey
{
    public string? Title { get; set; }
    public string Body { get; set; } = default!;
    public DateTime SendDate { get; set; }
    public Guid SenderId { get; set; }
    public Active Active { get; set; } = default!;
}
