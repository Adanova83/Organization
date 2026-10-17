namespace Library.Repository;

public interface IEventSubscription : IRepository<Domain.EventSubscription>
{
    Task<RepositoryResponseRange<Domain.EventSubscription>> SearchAsync(Guid? eventId, Guid? memberId, int page, int pageSize, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, (int Confirmed, int Waiting)>> CountByTargetAsync(IReadOnlyCollection<Guid> targetIds, CancellationToken ct);
    Task<Domain.EventSubscription?> PromoteNextAsync(Guid targetId, CancellationToken ct);
}
