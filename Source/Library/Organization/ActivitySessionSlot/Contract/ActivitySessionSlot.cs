namespace Library.Contract;

public class ActivitySessionSlot : PrimaryKey
{
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid SessionId { get; set; }
    public Guid? SpaceId { get; set; }
}
