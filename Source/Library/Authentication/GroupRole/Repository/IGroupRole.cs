namespace Library.Repository;

public interface IGroupRole : IRepository<Domain.GroupRole>
{
    Task<RepositoryResponseRange<Domain.GroupRole>> SearchByRoleIdAsync(Guid RoleId, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.GroupRole>> SearchByGroupIdAsync(Guid GroupId, CancellationToken ct);
}
