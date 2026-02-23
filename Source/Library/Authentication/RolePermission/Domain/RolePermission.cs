namespace Library.Domain;

public sealed class RolePermission : PrimaryKey
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
