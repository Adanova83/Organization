using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ProjectSlot", Schema = "Organization")]
internal class ProjectSlot : PrimaryKey
{
    [Required] public int DayOfWeek { get; set; }
    [Required] public TimeOnly StartTime { get; set; }
    [Required] public TimeOnly EndTime { get; set; }
    [Required] public Guid ProjectId { get; set; }
    [ForeignKey(nameof(ProjectId))] public Project Project { get; set; } = default!;
    public Guid? SpaceId { get; set; }
    [ForeignKey(nameof(SpaceId))] public Space? Space { get; set; } = null;
    public Audit Prs { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
