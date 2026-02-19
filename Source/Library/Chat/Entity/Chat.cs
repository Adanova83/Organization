using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Chat", Schema = "Chat")]
internal class Chat : PrimaryKey
{
    public string? Title { get; set; }
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    public Audit Doc { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
