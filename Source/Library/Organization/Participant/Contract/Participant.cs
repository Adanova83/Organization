namespace Library.Contract;

/// <summary>Contact d'une famille, tel que le voit l'animateur.</summary>
public sealed class ParticipantContact
{
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = "";
    public ContactRelation Relation { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsEmergencyContact { get; set; }
    public bool CanPickUp { get; set; }
}

/// <summary>
/// Participant d'un créneau, d'un événement ou d'un créneau de projet : ce dont l'animateur a besoin pour encadrer
/// (coordonnées, âge, contacts des familles), sans le reste de la fiche du membre.
/// </summary>
public sealed class Participant
{
    public Guid MemberId { get; set; }
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = "";
    public DateTime? Birthdate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsWaiting { get; set; }
    /// <summary>Rang dans la liste d'attente (1 = le prochain).</summary>
    public int? WaitingRank { get; set; }
    public List<ParticipantContact> Contacts { get; set; } = [];
}
