using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Role", Schema = "Authentication")]
[Index(nameof(Value), IsUnique = true)]
public class Role : PrimaryKey
{
    [Required, MaxLength(50)] public string Type { get; set; } = default!;
    [Required, MaxLength(200)] public string Value { get; set; } = default!;
}
