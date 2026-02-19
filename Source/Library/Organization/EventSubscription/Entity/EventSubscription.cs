using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("EventSubscription", Schema = "Organization")]
internal class EventSubscription : PrimaryKey
{
    [Required] public Guid EventId { get; set; }
    [ForeignKey(nameof(EventId))] public Event Event { get; set; } = default!;
    [Required] public Guid MemberId{ get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    public Audit Evs { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
