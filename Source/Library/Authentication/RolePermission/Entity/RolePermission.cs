using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("RolePermission", Schema = "Authentication")]
public class RolePermission : PrimaryKey
{
    [Required] public Guid RoleId { get; set; }
    [ForeignKey(nameof(RoleId))] public Role Role { get; set; } = default!;
    [Required] public Guid PermissionId { get; set; }
    [ForeignKey(nameof(PermissionId))] public Permission Permission { get; set; } = default!;
}
