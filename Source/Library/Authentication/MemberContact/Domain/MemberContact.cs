namespace Library.Domain;

public enum ContactRelation
{
    Mother,
    Father,
    Guardian,
    Other
}

public class MemberContact : PrimaryKey
{
    public Guid MemberId { get; set; } = Guid.Empty;
    public Guid ContactId { get; set; } = Guid.Empty;
    public ContactRelation Relation { get; set; } = ContactRelation.Other;
    public bool IsEmergencyContact { get; set; }
    public bool CanPickUp { get; set; }

    // Lecture seule : fiche du contact.
    public Contact? Contact { get; set; }
}
