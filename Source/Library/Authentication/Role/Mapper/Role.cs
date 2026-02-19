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
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Birthdate = source.Birthdate,
            Addr = source.Addr.DomainToContract(),
            Email = source.Email,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Role DomainToEntity(this Do.Role source)
    {
        return new En.Role
        {
            Id = source.Id,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Birthdate = source.Birthdate,
            Addr = source.Addr.DomainToEntity(),
            Email = source.Email,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Role EntityToDomain(this En.Role source)
    {
        return new Do.Role
        {
            Id = source.Id,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Birthdate = source.Birthdate,
            Addr = source.Addr.EntityToDomain(),
            Email = source.Email,
            Active = source.Active.EntityToDomain()
        };
    }
}
