using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("BadgeAssignation", Schema = "Access")]
internal class BadgeAssignation : PrimaryKey
{
    [Required] public Guid BadgeId { get; set; }
    [ForeignKey(nameof(BadgeId))] public Badge Badge { get; set; } = null!;
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = null!;
}
