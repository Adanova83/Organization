using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("OrganizationSession", Schema = "Organization")]
internal class OrganizationSession : PrimaryKey
{
    [Required] public Guid SchoolYearId { get; set; }
    [ForeignKey(nameof(SchoolYearId))] public SchoolYear SchoolYear { get; set; } = default!;
    [Required] public float Price { get; set; } = 0.0f;
    [Required] public Guid OrganizationId { get; set; }
    [ForeignKey(nameof(OrganizationId))] public Organization Organization { get; set; } = default!;
}
