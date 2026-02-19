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
            OrganizationSessionId = source.OrganizationSessionId,
            SpaceId = source.SpaceId,
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
            OrganizationSessionId = source.OrganizationSessionId,
            SpaceId = source.SpaceId,
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
            OrganizationSessionId = source.OrganizationSessionId,
            SpaceId = source.SpaceId,
            StartDate = source.StartDate,
            Active = source.Active.EntityToDomain()
        };
    }
}
