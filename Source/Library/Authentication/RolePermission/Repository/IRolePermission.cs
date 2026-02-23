namespace Library.Repository;

public interface IRolePermission : IRepository<Domain.RolePermission>
{
    Task<RepositoryResponseRange<Domain.RolePermission>> SearchByPermissionIdAsync(Guid permissionId, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.RolePermission>> SearchByRoleIdAsync(Guid roleId, CancellationToken ct);
}
