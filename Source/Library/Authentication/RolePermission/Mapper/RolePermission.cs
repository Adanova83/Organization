using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class RolePermission
{
    public static Ct.RolePermission DomainToContract(this Do.RolePermission source)
    {
        return new Ct.RolePermission
        {
            Id = source.Id,
            PermissionId = source.PermissionId,
            RoleId = source.RoleId,
            Audit = source.Audit.DomainToContract(),
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.RolePermission DomainToEntity(this Do.RolePermission source)
    {
        return new En.RolePermission
        {
            Id = source.Id,
            PermissionId = source.PermissionId,
            RoleId = source.RoleId,
            Audit = source.Audit.DomainToEntity(),
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.RolePermission EntityToDomain(this En.RolePermission source)
    {
        return new Do.RolePermission
        {
            Id = source.Id,
            PermissionId = source.PermissionId,
            RoleId = source.RoleId,
            Audit = source.Audit.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }
}
