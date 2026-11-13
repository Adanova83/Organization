using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class MemberContact
{
    public static Ct.MemberContact DomainToContract(this Do.MemberContact source)
    {
        return new Ct.MemberContact
        {
            Id = source.Id,
            MemberId = source.MemberId,
            ContactId = source.ContactId,
            Relation = (Ct.ContactRelation)source.Relation,
            IsEmergencyContact = source.IsEmergencyContact,
            CanPickUp = source.CanPickUp,
            ContactFirstname = source.Contact?.Firstname,
            ContactLastname = source.Contact?.Lastname,
            ContactPhoneNumber = source.Contact?.PhoneNumber,
            ContactEmail = source.Contact?.Email,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.MemberContact DomainToEntity(this Do.MemberContact source)
    {
        return new En.MemberContact
        {
            Id = source.Id,
            MemberId = source.MemberId,
            ContactId = source.ContactId,
            Relation = source.Relation,
            IsEmergencyContact = source.IsEmergencyContact,
            CanPickUp = source.CanPickUp,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.MemberContact EntityToDomain(this En.MemberContact source)
    {
        return new Do.MemberContact
        {
            Id = source.Id,
            MemberId = source.MemberId,
            ContactId = source.ContactId,
            Relation = source.Relation,
            IsEmergencyContact = source.IsEmergencyContact,
            CanPickUp = source.CanPickUp,
            // Fiche du contact, si elle a été chargée avec le lien.
            Contact = source.Contact?.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.MemberContact ContractToDomain(this Ct.MemberContact source, Guid id)
    {
        return new Do.MemberContact
        {
            Id = id,
            MemberId = source.MemberId,
            ContactId = source.ContactId,
            Relation = (Do.ContactRelation)source.Relation,
            IsEmergencyContact = source.IsEmergencyContact,
            CanPickUp = source.CanPickUp,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
