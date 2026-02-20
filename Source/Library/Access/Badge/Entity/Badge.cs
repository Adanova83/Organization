using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Badge", Schema = "Access")]
[Index(nameof(Identifier), IsUnique = true)]
internal class Badge : PrimaryKey
{
    [Required] public string Identifier { get; set; } = null!;
    public Audit Audit { get; set; } = null!;
    public Active Active { get; set; } = null!;
}
