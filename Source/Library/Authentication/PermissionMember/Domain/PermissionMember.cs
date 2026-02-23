namespace Library.Domain;

public sealed class PermissionMember : PrimaryKey
{
    public Guid PermissionId { get; set; }
    public Guid MemberId { get; set; }
}
