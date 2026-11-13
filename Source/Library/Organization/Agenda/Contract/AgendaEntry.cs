namespace Library.Contract;

/// <summary>Rôle d'un membre dans une séance de son agenda.</summary>
public enum AgendaRole
{
    Participant,
    Animator
}

/// <summary>
/// Une séance datée de l'agenda d'un membre : un créneau d'atelier ou de projet à une date, ou un événement, auquel il
/// est inscrit (ou en liste d'attente) ou qu'il anime.
/// </summary>
public sealed class AgendaEntry
{
    public SubscriptionTargetKind Kind { get; set; }
    /// <summary>Créneau (atelier ou projet) ou événement.</summary>
    public Guid TargetId { get; set; }
    /// <summary>Activité, projet ou événement auquel appartient la séance (pour le lien vers sa fiche).</summary>
    public Guid ParentId { get; set; }
    public DateOnly Date { get; set; }
    /// <summary>Horaires (heure de Paris).</summary>
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Title { get; set; } = "";
    public string? SpaceName { get; set; }
    public AgendaRole Role { get; set; }
    /// <summary>Participant en liste d'attente (pas encore inscrit).</summary>
    public bool IsWaiting { get; set; }
}
