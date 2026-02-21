using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("EventAnimation", Schema = "Organization")]
internal class EventAnimation : PrimaryKey
{
    [Required] public Guid EventId { get; set; }
    [ForeignKey(nameof(EventId))] public Event Event { get; set; } = default!;
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    public int Order { get; set; }
}
