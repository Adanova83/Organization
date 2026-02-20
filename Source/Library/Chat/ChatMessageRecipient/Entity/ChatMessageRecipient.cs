using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ChatMessageRecipient", Schema = "Chat")]
internal class ChatMessageRecipient : PrimaryKey
{
    public DateTime? ReceiveDate { get; set; }
    public DateTime? ReadDate { get; set; }
    [Required] public Guid ChatMessageId { get; set; }
    [ForeignKey(nameof(ChatMessageId))] public ChatMessage ChatMessage { get; set; } = default!;
    [Required] public Guid RecipentId { get; set; }
    [ForeignKey(nameof(RecipentId))] public Member Recipent { get; set; } = default!;
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
