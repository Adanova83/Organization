using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Country", Schema = "Authentication")]
[Index(nameof(CodeAlpha2), IsUnique = true)]
[Index(nameof(CodeAlpha3), IsUnique = true)]
public class Country : PrimaryKey
{
    [Required, MaxLength(2)] public string CodeAlpha2 { get; set; } = default!;
    [Required, MaxLength(3)] public string CodeAlpha3 { get; set; } = default!;
    [MaxLength(100)] public string Name { get; set; } = default!;
}
