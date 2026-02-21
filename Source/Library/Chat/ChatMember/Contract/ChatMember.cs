namespace Library.Contract;

public class ChatMember : PrimaryKey
{
    public bool IsAdmin { get; set; } = false;
    public Guid ChatId { get; set; }
    public Guid MemberId { get; set; }
}
