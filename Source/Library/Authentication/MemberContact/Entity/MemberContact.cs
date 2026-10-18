using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("MemberContact", Schema = "Authentication")]
[Index(nameof(MemberId), nameof(ContactId), IsUnique = true)]
internal class MemberContact : PrimaryKey
{
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = null!;
    [Required] public Guid ContactId { get; set; }
    [ForeignKey(nameof(ContactId))] public Contact Contact { get; set; } = null!;
    [Required] public Domain.ContactRelation Relation { get; set; }
    [Required] public bool IsEmergencyContact { get; set; }
    [Required] public bool CanPickUp { get; set; }
}
