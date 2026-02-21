namespace Library.Repository;

public interface IGroupMember : IRepository<Domain.GroupMember>
{
    Task<RepositoryResponseRange<Domain.GroupMember>> SearchByMemberIdAsync(Guid memberId, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.GroupMember>> SearchByGroupIdAsync(Guid GroupId, CancellationToken ct);
}
