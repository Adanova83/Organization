using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Group", Schema = "Authentication")]
[Index(nameof(Code), IsUnique = true)]
public class Group : PrimaryKey
{
    [Required, MaxLength(50)] public string Code { get; set; } = default!;
    [Required, MaxLength(200)] public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    [Required, MaxLength(8)] public string RGBColor { get; set; } = "FFFFFFFF";
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}

