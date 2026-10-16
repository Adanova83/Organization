using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Event", Schema = "Organization")]
[Index(nameof(Code), IsUnique = true)]
[Index(nameof(SeriesId))]
internal class Event : PrimaryKey
{
    [Required, MaxLength(50)] public string Code { get; set; } = default!;
    [Required, MaxLength(100)] public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Picture { get; set; }
    [Required, MaxLength(8)] public string RGBColor { get; set; } = "FFFFFFFF";
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    public int? AgeMinimum { get; set; } = null;
    [Required] public Guid OrganizationId { get; set; }
    [ForeignKey(nameof(OrganizationId))] public Organization Organization { get; set; } = default!;
    [Required] public Guid SchoolYearId { get; set; }
    [ForeignKey(nameof(SchoolYearId))] public SchoolYear SchoolYear { get; set; } = default!;
    public Guid? SpaceId { get; set; }
    public int? Capacity { get; set; }
    public Guid? SeriesId { get; set; }
    [ForeignKey(nameof(SpaceId))] public Space? Space { get; set; } = null;
}
