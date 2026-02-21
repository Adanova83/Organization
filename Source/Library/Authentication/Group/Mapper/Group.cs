using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Group
{
    public static Ct.Group DomainToContract(this Do.Group source)
    {
        return new Ct.Group
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Audit = source.Audit.DomainToContract(),
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Group DomainToEntity(this Do.Group source)
    {
        return new En.Group
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Audit = source.Audit.DomainToEntity(),
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Group EntityToDomain(this En.Group source)
    {
        return new Do.Group
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Audit = source.Audit.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }
}
