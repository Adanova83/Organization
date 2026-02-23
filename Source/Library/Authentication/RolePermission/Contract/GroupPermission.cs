namespace Library.Contract;

public sealed class RolePermission : PrimaryKey
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
