namespace Library.Repository;

public interface IProjectSlotSubscription : IRepository<Domain.ProjectSlotSubscription>
{
    Task<RepositoryResponseRange<Domain.ProjectSlotSubscription>> SearchAsync(Guid? projectSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, (int Confirmed, int Waiting)>> CountByTargetAsync(IReadOnlyCollection<Guid> targetIds, CancellationToken ct);
    Task<Domain.ProjectSlotSubscription?> PromoteNextAsync(Guid targetId, CancellationToken ct);
}
