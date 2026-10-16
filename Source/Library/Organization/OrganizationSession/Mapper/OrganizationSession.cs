using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class OrganizationSession
{
    public static Ct.OrganizationSession DomainToContract(this Do.OrganizationSession source)
    {
        return new Ct.OrganizationSession
        {
            Id = source.Id,
            SchoolYearId = source.SchoolYearId,
            SchoolYearCode = source.SchoolYearCode,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            OrganizationId = source.OrganizationId,
            Price = source.Price,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.OrganizationSession DomainToEntity(this Do.OrganizationSession source)
    {
        return new En.OrganizationSession
        {
            Id = source.Id,
            SchoolYearId = source.SchoolYearId,
            OrganizationId = source.OrganizationId,
            Price = source.Price,
            Active = source.Active.DomainToEntity()
        };
    }

    /// <summary>Entité vers domaine ; les dates viennent de l'année scolaire si elle a été chargée.</summary>
    internal static Do.OrganizationSession EntityToDomain(this En.OrganizationSession source)
    {
        return new Do.OrganizationSession
        {
            Id = source.Id,
            SchoolYearId = source.SchoolYearId,
            SchoolYearCode = source.SchoolYear?.Code,
            StartDate = source.SchoolYear?.StartDate,
            EndDate = source.SchoolYear?.EndDate,
            OrganizationId = source.OrganizationId,
            Price = source.Price,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client ; les dates viennent de l'année.</summary>
    public static Do.OrganizationSession ContractToDomain(this Ct.OrganizationSession source, Guid id)
    {
        return new Do.OrganizationSession
        {
            Id = id,
            SchoolYearId = source.SchoolYearId,
            Price = source.Price,
            OrganizationId = source.OrganizationId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
