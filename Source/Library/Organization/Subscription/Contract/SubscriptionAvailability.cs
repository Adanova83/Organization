namespace Library.Contract;

/// <summary>Sorte d'élément auquel on s'inscrit.</summary>
public enum SubscriptionTargetKind
{
    ActivitySlot,
    Event,
    ProjectSlot
}

/// <summary>Places d'un élément (créneau ou événement) : capacité, inscrits et liste d'attente, sans les noms.</summary>
public class SubscriptionAvailability
{
    public Guid TargetId { get; set; }

    /// <summary>Vide : illimitée.</summary>
    public int? Capacity { get; set; }

    public int Subscribed { get; set; }
    public int Waiting { get; set; }

    /// <summary>Places restantes ; vide si la capacité est illimitée.</summary>
    public int? Remaining => Capacity is { } places ? Math.Max(places - Subscribed, 0) : null;

    public bool IsFull => Capacity is { } places && Subscribed >= places;
}
