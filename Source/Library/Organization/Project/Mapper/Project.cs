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
            Active = source.Active.EntityToDomain()
        };
    }
}
