namespace Library.Contract;

public class ChatMessageRecipient : PrimaryKey
{
    public DateTime? ReceiveDate { get; set; }
    public DateTime? ReadDate { get; set; }
    public Guid ChatMessageId { get; set; }
    public Guid RecipentId { get; set; }
}
