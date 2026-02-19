namespace Library.Repository;

public interface IProjectSlotSubscription : IRepository<Domain.ProjectSlotSubscription>
{
    Task<RepositoryResponseRange<Domain.ProjectSlotSubscription>> SearchAsync(Guid? projectSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct);
}
