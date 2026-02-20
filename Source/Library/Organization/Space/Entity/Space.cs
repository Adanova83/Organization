using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Space", Schema = "Organization")]
internal class Space : PrimaryKey
{
    [Required, MaxLength(100)] public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    [Required, MaxLength(8)] public string RGBColor { get; set; } = "FFFFFFFF";
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
