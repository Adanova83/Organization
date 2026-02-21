using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class RoleMember
{
    public static Ct.RoleMember DomainToContract(this Do.RoleMember source)
    {
        return new Ct.RoleMember
        {
            Id = source.Id,
            MemberId = source.MemberId,
            RoleId = source.RoleId,
            Audit = source.Audit.DomainToContract(),
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.RoleMember DomainToEntity(this Do.RoleMember source)
    {
        return new En.RoleMember
        {
            Id = source.Id,
            MemberId = source.MemberId,
            RoleId = source.RoleId,
            Audit = source.Audit.DomainToEntity(),
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.RoleMember EntityToDomain(this En.RoleMember source)
    {
        return new Do.RoleMember
        {
            Id = source.Id,
            MemberId = source.MemberId,
            RoleId = source.RoleId,
            Audit = source.Audit.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }
}
