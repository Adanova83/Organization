namespace Library.Domain;

public sealed class GroupMember : PrimaryKey
{
    public Guid GroupId { get; set; }
    public Guid MemberId { get; set; }
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
