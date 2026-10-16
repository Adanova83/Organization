using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ActivitySession", Schema = "Organization")]
internal class ActivitySession : PrimaryKey
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [Required] public bool DuringTermTime { get; set; } = true;
    [Required] public bool DuringSchoolHolidays { get; set; }
    [Required] public float Price { get; set; } = 0.0f;
    [Required] public Guid ActivityId { get; set; }
    [ForeignKey(nameof(ActivityId))] public Activity Activity { get; set; } = default!;
    [Required] public Guid SchoolYearId { get; set; }
    [ForeignKey(nameof(SchoolYearId))] public SchoolYear SchoolYear { get; set; } = default!;
}
