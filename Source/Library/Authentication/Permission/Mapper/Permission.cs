using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Permission
{
    public static Ct.Permission DomainToContract(this Do.Permission source)
    {
        return new Ct.Permission
        {
            Id = source.Id,
            Type = source.Type,
            Value = source.Value,
            Audit = source.Audit.DomainToContract(),
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Permission DomainToEntity(this Do.Permission source)
    {
        return new En.Permission
        {
            Id = source.Id,
            Type = source.Type,
            Value = source.Value,
            Audit = source.Audit.DomainToEntity(),
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Permission EntityToDomain(this En.Permission source)
    {
        return new Do.Permission
        {
            Id = source.Id,
            Type = source.Type,
            Value = source.Value,
            Audit = source.Audit.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }
}
