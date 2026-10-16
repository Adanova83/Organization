using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Contribution
{
    public static Ct.Contribution DomainToContract(this Do.Contribution source)
    {
        return new Ct.Contribution
        {
            Id = source.Id,
            MemberId = source.MemberId,
            OrganizationSessionId = source.OrganizationSessionId,
            PayedDate = source.PayedDate,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Contribution DomainToEntity(this Do.Contribution source)
    {
        return new En.Contribution
        {
            Id = source.Id,
            MemberId = source.MemberId,
            OrganizationSessionId = source.OrganizationSessionId,
            PayedDate = source.PayedDate,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Contribution EntityToDomain(this En.Contribution source)
    {
        return new Do.Contribution
        {
            Id = source.Id,
            MemberId = source.MemberId,
            OrganizationSessionId = source.OrganizationSessionId,
            PayedDate = source.PayedDate,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Contribution ContractToDomain(this Ct.Contribution source, Guid id)
    {
        return new Do.Contribution
        {
            Id = id,
            MemberId = source.MemberId,
            OrganizationSessionId = source.OrganizationSessionId,
            PayedDate = source.PayedDate,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
