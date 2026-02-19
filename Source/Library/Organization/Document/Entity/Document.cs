using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Document", Schema = "Organization")]
internal class Document : PrimaryKey
{
    [Required] public string Title { get; set; } = default!;
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    public Audit Doc { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
