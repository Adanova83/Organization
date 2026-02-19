using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("ActivitySessionSlotAnimation", Schema = "Organization")]
internal class ActivitySessionSlotAnimation : PrimaryKey
{
    [Required] public Guid SessionSlotId { get; set; }
    [ForeignKey(nameof(SessionSlotId))] public ActivitySessionSlot SessionSlot { get; set; } = default!;
    [Required] public Guid MemberId{ get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    public int Order { get; set; }
    public Audit Asa { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
