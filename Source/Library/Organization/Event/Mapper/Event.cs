using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Event
{
    public static Ct.Event DomainToContract(this Do.Event source)
    {
        return new Ct.Event
        {
            Id = source.Id,
            AgeMinimum = source.AgeMinimum,
            Code = source.Code,
            Description = source.Description,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            EndDate = source.EndDate,
            Name = source.Name,
            OrganizationId = source.OrganizationId,
            SchoolYearId = source.SchoolYearId,
            SchoolYearCode = source.SchoolYearCode,
            SpaceId = source.SpaceId,
            SeriesId = source.SeriesId,
            StartDate = source.StartDate,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Event DomainToEntity(this Do.Event source)
    {
        return new En.Event
        {
            Id = source.Id,
            AgeMinimum = source.AgeMinimum,
            Code = source.Code,
            Description = source.Description,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            EndDate = source.EndDate,
            Name = source.Name,
            OrganizationId = source.OrganizationId,
            SchoolYearId = source.SchoolYearId,
            SpaceId = source.SpaceId,
            SeriesId = source.SeriesId,
            StartDate = source.StartDate,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Event EntityToDomain(this En.Event source)
    {
        return new Do.Event
        {
            Id = source.Id,
            AgeMinimum = source.AgeMinimum,
            Code = source.Code,
            Description = source.Description,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            EndDate = source.EndDate,
            Name = source.Name,
            OrganizationId = source.OrganizationId,
            SchoolYearId = source.SchoolYearId,
            SchoolYearCode = source.SchoolYear?.Code,
            SpaceId = source.SpaceId,
            SeriesId = source.SeriesId,
            StartDate = source.StartDate,
            Active = source.Active.EntityToDomain(),
            Audit = source.Audit.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Event ContractToDomain(this Ct.Event source, Guid id)
    {
        return new Do.Event
        {
            Id = id,
            Code = source.Code,
            Name = source.Name,
            Description = source.Description,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            AgeMinimum = source.AgeMinimum,
            OrganizationId = source.OrganizationId,
            SchoolYearId = source.SchoolYearId,
            SpaceId = source.SpaceId,
            SeriesId = source.SeriesId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
