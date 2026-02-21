namespace Library.Domain;

public class ProjectSlotAnimation : PrimaryKey
{
    public Guid ProjectSlotId { get; set; }
    public Guid MemberId { get; set; }
    public int Order { get; set; }
}
