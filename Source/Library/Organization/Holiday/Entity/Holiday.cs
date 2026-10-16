using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Holiday", Schema = "Organization")]
internal class Holiday : PrimaryKey
{
    [Required] public string Name { get; set; } = default!;
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    public Guid? SchoolYearId { get; set; }
    [ForeignKey(nameof(SchoolYearId))] public SchoolYear? SchoolYear { get; set; }
}
