namespace Library.Repository;

public interface IActivitySessionSlotSubscription : IRepository<Domain.ActivitySessionSlotSubscription>
{
    Task<RepositoryResponseRange<Domain.ActivitySessionSlotSubscription>> SearchAsync(Guid? sessionSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, (int Confirmed, int Waiting)>> CountByTargetAsync(IReadOnlyCollection<Guid> targetIds, CancellationToken ct);
    Task<Domain.ActivitySessionSlotSubscription?> PromoteNextAsync(Guid targetId, CancellationToken ct);
}
