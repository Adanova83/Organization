namespace Library.Contract;

public sealed class GroupMember : PrimaryKey
{
    public Guid GroupId { get; set; }
    public Guid MemberId { get; set; }
}
