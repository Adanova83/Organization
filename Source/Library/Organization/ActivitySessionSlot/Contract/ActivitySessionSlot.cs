namespace Library.Contract;

public class ActivitySessionSlot : PrimaryKey
{
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    /// <summary>Intervalle entre deux occurrences, en semaines : 1 = chaque semaine, 2 = une semaine sur deux.</summary>
    public int Frequency { get; set; } = 1;
    public Guid SessionId { get; set; }
    public Guid? SpaceId { get; set; }

    /// <summary>Nombre maximal d'inscrits ; vide = illimité.</summary>
    public int? Capacity { get; set; }
}
