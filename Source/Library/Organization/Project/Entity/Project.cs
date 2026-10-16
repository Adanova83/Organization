using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Project", Schema = "Organization")]
[Index(nameof(Code), IsUnique = true)]
internal class Project : PrimaryKey
{
    [Required, MaxLength(50)] public string Code { get; set; } = default!;
    [Required, MaxLength(100)] public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Picture { get; set; }
    [Required, MaxLength(8)] public string RGBColor { get; set; } = "FFFFFFFF";
    [Required] public int AgeMinimum { get; set; } = 0;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [Required] public Guid SchoolYearId { get; set; }
    [ForeignKey(nameof(SchoolYearId))] public SchoolYear SchoolYear { get; set; } = default!;
    [Required] public Guid OrganizationId { get; set; }
    [ForeignKey(nameof(OrganizationId))] public Organization Organization { get; set; } = default!;
}
