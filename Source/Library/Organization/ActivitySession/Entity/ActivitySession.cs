using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ActivitySession", Schema = "Organization")]
internal class ActivitySession : PrimaryKey
{
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    [Required] public float Price { get; set; } = 0.0f;
    [Required] public Guid ActivityId { get; set; }
    [ForeignKey(nameof(ActivityId))] public Activity Activity { get; set; } = default!;
    [Required] public Guid OrganizationSessionId { get; set; }
    [ForeignKey(nameof(OrganizationSessionId))] public OrganizationSession OrganizationSession { get; set; } = default!;
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
