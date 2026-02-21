using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("RoleMember", Schema = "Authentication")]
public class RoleMember : PrimaryKey
{
    [Required] public Guid RoleId { get; set; }
    [ForeignKey(nameof(RoleId))] public Role Role { get; set; } = default!;
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
}
