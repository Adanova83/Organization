using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class GroupRole
{
    public static Ct.GroupRole DomainToContract(this Do.GroupRole source)
    {
        return new Ct.GroupRole
        {
            Id = source.Id,
            RoleId = source.RoleId,
            GroupId = source.GroupId,
            Audit = source.Audit.DomainToContract(),
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.GroupRole DomainToEntity(this Do.GroupRole source)
    {
        return new En.GroupRole
        {
            Id = source.Id,
            RoleId = source.RoleId,
            GroupId = source.GroupId,
            Audit = source.Audit.DomainToEntity(),
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.GroupRole EntityToDomain(this En.GroupRole source)
    {
        return new Do.GroupRole
        {
            Id = source.Id,
            RoleId = source.RoleId,
            GroupId = source.GroupId,
            Audit = source.Audit.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }
}
