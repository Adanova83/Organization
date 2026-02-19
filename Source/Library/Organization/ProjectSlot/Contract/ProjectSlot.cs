namespace Library.Contract;

public class ProjectSlot : PrimaryKey
{
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SpaceId { get; set; }
    public Active Active { get; set; } = default!;
}
