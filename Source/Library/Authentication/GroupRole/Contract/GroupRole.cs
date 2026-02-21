namespace Library.Contract;

public sealed class GroupRole : PrimaryKey
{
    public Guid GroupId { get; set; }
    public Guid RoleId { get; set; }
}
