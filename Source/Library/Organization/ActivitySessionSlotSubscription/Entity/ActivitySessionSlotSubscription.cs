using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ActivitySessionSlotSubscription", Schema = "Organization")]
internal class ActivitySessionSlotSubscription : PrimaryKey
{
    [Required] public Guid SessionSlotId { get; set; }
    [ForeignKey(nameof(SessionSlotId))] public ActivitySessionSlot SessionSlot { get; set; } = default!;
    [Required] public Guid MemberId{ get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    public Audit Ass { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
