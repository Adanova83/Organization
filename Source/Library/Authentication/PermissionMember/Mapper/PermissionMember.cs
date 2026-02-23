using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class PermissionMember
{
    public static Ct.PermissionMember DomainToContract(this Do.PermissionMember source)
    {
        return new Ct.PermissionMember
        {
            Id = source.Id,
            MemberId = source.MemberId,
            PermissionId = source.PermissionId,
            Audit = source.Audit.DomainToContract(),
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.PermissionMember DomainToEntity(this Do.PermissionMember source)
    {
        return new En.PermissionMember
        {
            Id = source.Id,
            MemberId = source.MemberId,
            PermissionId = source.PermissionId,
            Audit = source.Audit.DomainToEntity(),
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.PermissionMember EntityToDomain(this En.PermissionMember source)
    {
        return new Do.PermissionMember
        {
            Id = source.Id,
            MemberId = source.MemberId,
            PermissionId = source.PermissionId,
            Audit = source.Audit.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }
}
