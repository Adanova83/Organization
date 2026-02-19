using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ChatMessage", Schema = "Chat")]
internal class ChatMessage : PrimaryKey
{
    public string? Title { get; set; }
    [Required] public string Body { get; set; } = default!;
    [Required] public DateTime SendDate { get; set; }
    [Required] public Guid SenderId { get; set; }
    [ForeignKey(nameof(SenderId))] public Member Sender { get; set; } = default!;
    public Audit Chm { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
