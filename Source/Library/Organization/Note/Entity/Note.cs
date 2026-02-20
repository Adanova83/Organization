using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Note", Schema = "Organization")]
internal class Note : PrimaryKey
{
    [Required, MaxLength(200)] public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    [Required, MaxLength(8)] public string RGBBackgroundColor { get; set; } = "FFFFFFFF";
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    public Audit Audit { get; set; } = default!;
}
