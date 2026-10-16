using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Holiday
{
    public static Ct.Holiday DomainToContract(this Do.Holiday source)
    {
        return new Ct.Holiday
        {
            Id = source.Id,
            EndDate = source.EndDate,
            Name = source.Name,
            StartDate = source.StartDate,
            SchoolYearId = source.SchoolYearId,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Holiday DomainToEntity(this Do.Holiday source)
    {
        return new En.Holiday
        {
            Id = source.Id,
            EndDate = source.EndDate,
            Name = source.Name,
            StartDate = source.StartDate,
            SchoolYearId = source.SchoolYearId,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Holiday EntityToDomain(this En.Holiday source)
    {
        return new Do.Holiday
        {
            Id = source.Id,
            EndDate = source.EndDate,
            Name = source.Name,
            StartDate = source.StartDate,
            SchoolYearId = source.SchoolYearId,
            Active = source.Active.EntityToDomain(),
            Audit = source.Audit.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Holiday ContractToDomain(this Ct.Holiday source, Guid id)
    {
        return new Do.Holiday
        {
            Id = id,
            Name = source.Name,
            StartDate = source.StartDate,
            SchoolYearId = source.SchoolYearId,
            EndDate = source.EndDate,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
