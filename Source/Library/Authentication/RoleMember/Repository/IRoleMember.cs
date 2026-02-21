namespace Library.Repository;

public interface IRoleMember : IRepository<Domain.RoleMember>
{
    Task<RepositoryResponseRange<Domain.RoleMember>> SearchByMemberIdAsync(Guid memberId, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.RoleMember>> SearchByRoleIdAsync(Guid roleId, CancellationToken ct);
}
