namespace Library.Domain;

public sealed class RoleMember : PrimaryKey
{
    public Guid RoleId { get; set; }
    public Guid MemberId { get; set; }
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
