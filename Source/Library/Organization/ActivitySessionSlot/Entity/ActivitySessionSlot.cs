using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ActivitySessionSlot", Schema = "Organization")]
internal class ActivitySessionSlot : PrimaryKey
{
    [Required] public int DayOfWeek { get; set; }
    [Required] public TimeOnly StartTime { get; set; }
    [Required] public TimeOnly EndTime { get; set; }
    [Required] public int Frequency { get; set; }
    [Required] public Guid SessionId { get; set; }
    [ForeignKey(nameof(SessionId))] public ActivitySession Session { get; set; } = default!;
    [Required] public Guid? SpaceId { get; set; }
    [ForeignKey(nameof(SpaceId))] public Space? Space { get; set; } = null;
}
