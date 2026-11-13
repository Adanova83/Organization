using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Contact
{
    public static Ct.Contact DomainToContract(this Do.Contact source)
    {
        return new Ct.Contact
        {
            Id = source.Id,
            Firstname = source.Firstname,
            Lastname = source.Lastname,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Contact DomainToEntity(this Do.Contact source)
    {
        return new En.Contact
        {
            Id = source.Id,
            Firstname = source.Firstname,
            Lastname = source.Lastname,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Contact EntityToDomain(this En.Contact source)
    {
        return new Do.Contact
        {
            Id = source.Id,
            Firstname = source.Firstname,
            Lastname = source.Lastname,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Contact ContractToDomain(this Ct.Contact source, Guid id)
    {
        return new Do.Contact
        {
            Id = id,
            Firstname = source.Firstname,
            Lastname = source.Lastname,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
