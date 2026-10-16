using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("SchoolYear", Schema = "Organization")]
[Index(nameof(Code), IsUnique = true)]
internal class SchoolYear : PrimaryKey
{
    [Required, MaxLength(20)] public string Code { get; set; } = default!;
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    [Required, MaxLength(1)] public string Zone { get; set; } = "B";
}
