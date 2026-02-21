using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("GroupMember", Schema = "Authentication")]
public class GroupMember : PrimaryKey
{
    [Required] public Guid GroupId { get; set; }
    [ForeignKey(nameof(GroupId))] public Group Group { get; set; } = default!;
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
}
