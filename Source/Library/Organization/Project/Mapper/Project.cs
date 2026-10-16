using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Project
{
    public static Ct.Project DomainToContract(this Do.Project source)
    {
        return new Ct.Project
        {
            Id = source.Id,
            Name = source.Name,
            AgeMinimum = source.AgeMinimum,
            Code = source.Code,
            Description = source.Description,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            OrganizationId = source.OrganizationId,
            SchoolYearId = source.SchoolYearId,
            DuringTermTime = source.DuringTermTime,
            DuringSchoolHolidays = source.DuringSchoolHolidays,
            SchoolYearCode = source.SchoolYearCode,
            EffectiveStartDate = source.EffectiveStartDate,
            EffectiveEndDate = source.EffectiveEndDate,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Project DomainToEntity(this Do.Project source)
    {
        return new En.Project
        {
            Id = source.Id,
            Name = source.Name,
            AgeMinimum = source.AgeMinimum,
            Code = source.Code,
            Description = source.Description,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            OrganizationId = source.OrganizationId,
            SchoolYearId = source.SchoolYearId,
            DuringTermTime = source.DuringTermTime,
            DuringSchoolHolidays = source.DuringSchoolHolidays,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Project EntityToDomain(this En.Project source)
    {
        return new Do.Project
        {
            Id = source.Id,
            Name = source.Name,
            AgeMinimum = source.AgeMinimum,
            Code = source.Code,
            Description = source.Description,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            OrganizationId = source.OrganizationId,
            SchoolYearId = source.SchoolYearId,
            DuringTermTime = source.DuringTermTime,
            DuringSchoolHolidays = source.DuringSchoolHolidays,
            SchoolYearCode = source.SchoolYear?.Code,
            EffectiveStartDate = source.StartDate ?? source.SchoolYear?.StartDate,
            EffectiveEndDate = source.EndDate ?? source.SchoolYear?.EndDate,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Active = source.Active.EntityToDomain(),
            Audit = source.Audit.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Project ContractToDomain(this Ct.Project source, Guid id)
    {
        return new Do.Project
        {
            Id = id,
            Code = source.Code,
            Name = source.Name,
            Description = source.Description,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            AgeMinimum = source.AgeMinimum,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            OrganizationId = source.OrganizationId,
            SchoolYearId = source.SchoolYearId,
            DuringTermTime = source.DuringTermTime,
            DuringSchoolHolidays = source.DuringSchoolHolidays,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
