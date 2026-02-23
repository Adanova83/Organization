namespace Library.Repository;

public interface IPermissionMember : IRepository<Domain.PermissionMember>
{
    Task<RepositoryResponseRange<Domain.PermissionMember>> SearchByMemberIdAsync(Guid memberId, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.PermissionMember>> SearchByPermissionIdAsync(Guid permissionId, CancellationToken ct);
}
