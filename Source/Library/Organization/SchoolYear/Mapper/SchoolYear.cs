using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class SchoolYear
{
    public static Ct.SchoolYear DomainToContract(this Do.SchoolYear source)
    {
        return new Ct.SchoolYear
        {
            Id = source.Id,
            Code = source.Code,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Zone = source.Zone,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.SchoolYear DomainToEntity(this Do.SchoolYear source)
    {
        return new En.SchoolYear
        {
            Id = source.Id,
            Code = source.Code,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Zone = source.Zone,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.SchoolYear EntityToDomain(this En.SchoolYear source)
    {
        return new Do.SchoolYear
        {
            Id = source.Id,
            Code = source.Code,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Zone = source.Zone,
            Active = source.Active.EntityToDomain(),
            Audit = source.Audit.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.SchoolYear ContractToDomain(this Ct.SchoolYear source, Guid id)
    {
        return new Do.SchoolYear
        {
            Id = id,
            Code = source.Code,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Zone = source.Zone,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
