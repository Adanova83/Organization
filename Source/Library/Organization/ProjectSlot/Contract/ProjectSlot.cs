namespace Library.Contract;

public class ProjectSlot : PrimaryKey
{
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SpaceId { get; set; }

    /// <summary>Nombre maximal d'inscrits ; vide = illimité.</summary>
    public int? Capacity { get; set; }
}
