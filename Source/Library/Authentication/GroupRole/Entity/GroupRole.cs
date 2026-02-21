using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("GroupRole", Schema = "Authentication")]
public class GroupRole : PrimaryKey
{
    [Required] public Guid GroupId { get; set; }
    [ForeignKey(nameof(GroupId))] public Group Group { get; set; } = default!;
    [Required] public Guid RoleId { get; set; }
    [ForeignKey(nameof(RoleId))] public Role Role { get; set; } = default!;
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
