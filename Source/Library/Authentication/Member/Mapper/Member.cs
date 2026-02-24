using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Member
{
    public static Ct.Member DomainToContract(this Do.Member source)
    {
        return new Ct.Member
        {
            Id = source.Id,
            Firstname = source.Firstname,
            Lastname = source.Lastname,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Birthdate = source.Birthdate,
            Email = source.Email,
            AskedNewPassword = source.AskedNewPassword,
            HashedPassword = source.HashedPassword,
            PhoneNumber = source.PhoneNumber,
            Addr = source.Addr.DomainToContract(),
            Audit = source.Audit.DomainToContract(),
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Member DomainToEntity(this Do.Member source)
    {
        return new En.Member
        {
            Id = source.Id,
            Firstname = source.Firstname,
            Lastname = source.Lastname,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Birthdate = source.Birthdate,
            Email = source.Email,
            AskedNewPassword = source.AskedNewPassword,
            HashedPassword = source.HashedPassword,
            PhoneNumber = source.PhoneNumber,
            Addr = source.Addr.DomainToEntity(),
            Audit = source.Audit.DomainToEntity(),
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Member EntityToDomain(this En.Member source)
    {
        return new Do.Member
        {
            Id = source.Id,
            Firstname = source.Firstname,
            Lastname = source.Lastname,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Birthdate = source.Birthdate,
            Email = source.Email,
            AskedNewPassword = source.AskedNewPassword,
            HashedPassword = source.HashedPassword,
            PhoneNumber = source.PhoneNumber,
            Addr = source.Addr.EntityToDomain(),
            Audit = source.Audit.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }
}
