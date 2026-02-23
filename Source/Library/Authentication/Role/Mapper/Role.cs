using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Role
{
    public static Ct.Role DomainToContract(this Do.Role source)
    {
        return new Ct.Role
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

    internal static En.Role DomainToEntity(this Do.Role source)
    {
        return new En.Role
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

    internal static Do.Role EntityToDomain(this En.Role source)
    {
        return new Do.Role
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
