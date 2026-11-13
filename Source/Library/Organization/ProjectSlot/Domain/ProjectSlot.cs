namespace Library.Domain;

public class ProjectSlot : PrimaryKey
{
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SpaceId { get; set; }
    public int? Capacity { get; set; }

    /// <summary>Créneau ponctuel : sa seule date (le jour de la semaine en découle). Vide = chaque semaine, sur la période du projet.</summary>
    public DateOnly? Date { get; set; }
}
