using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ChatMember", Schema = "Chat")]
internal class ChatMember : PrimaryKey
{
    public bool IsAdmin { get; set; } = false;
    [Required] public Guid ChatId { get; set; }
    [ForeignKey(nameof(ChatId))] public Chat Chat { get; set; } = default!;
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
