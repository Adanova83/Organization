namespace Library.Contract;

/// <summary>Lien entre un contact et un membre (un même contact peut suivre plusieurs enfants).</summary>
public enum ContactRelation
{
    Mother,
    Father,
    Guardian,
    Other
}

/// <summary>Contact rattaché à un membre : lien de parenté, à prévenir en cas d'urgence, autorisé à venir chercher l'enfant.</summary>
public class MemberContact : PrimaryKey
{
    public Guid MemberId { get; set; } = Guid.Empty;
    public Guid ContactId { get; set; } = Guid.Empty;
    public ContactRelation Relation { get; set; } = ContactRelation.Other;
    public bool IsEmergencyContact { get; set; }
    public bool CanPickUp { get; set; }

    // Renseignés par l'API depuis la fiche du contact (lecture seule, ignorés à l'envoi).
    public string? ContactFirstname { get; set; }
    public string? ContactLastname { get; set; }
    public string? ContactPhoneNumber { get; set; }
    public string? ContactEmail { get; set; }
}
