namespace Library.Contract;

public sealed class RoleMember : PrimaryKey
{
    public Guid RoleId { get; set; }
    public Guid MemberId { get; set; }
}
