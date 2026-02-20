using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Chat", Schema = "Chat")]
internal class Chat : PrimaryKey
{
    public string? Title { get; set; }
    public bool IsGroup { get; set; } = false;
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
