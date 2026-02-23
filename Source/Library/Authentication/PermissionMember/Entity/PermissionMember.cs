using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("PermissionMember", Schema = "Authentication")]
public class PermissionMember : PrimaryKey
{
    [Required] public Guid PermissionId { get; set; }
    [ForeignKey(nameof(PermissionId))] public Permission Permission { get; set; } = default!;
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
}
