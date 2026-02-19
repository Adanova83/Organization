namespace Library.Contract;

public class ProjectSlotAnimation : PrimaryKey
{
    public Guid ProjectSlotId { get; set; }
    public Guid MemberId { get; set; }
    public int Order { get; set; }
    public Active Active { get; set; } = default!;
}
