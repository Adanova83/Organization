using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("NoteLink", Schema = "Organization")]
internal class NoteLink : PrimaryKey
{
    [Required] public Guid NoteId { get; set; }
    [ForeignKey(nameof(NoteId))] public Note Note { get; set; } = default!;
    public string LinkedType { get; set; } = default!;
    public Guid LinkedId { get; set; }
}
