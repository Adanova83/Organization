using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ProjectSlotSubscription", Schema = "Organization")]
internal class ProjectSlotSubscription : PrimaryKey
{
    [Required] public Guid ProjectSlotId { get; set; }
    [ForeignKey(nameof(ProjectSlotId))] public ProjectSlot ProjectSlot { get; set; } = default!;
    [Required] public Guid MemberId{ get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    public Audit Pss { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
