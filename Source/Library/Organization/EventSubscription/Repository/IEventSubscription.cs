namespace Library.Repository;

public interface IEventSubscription : IRepository<Domain.EventSubscription>
{
    Task<RepositoryResponseRange<Domain.EventSubscription>> SearchAsync(Guid? eventId, Guid? memberId, int page, int pageSize, CancellationToken ct);
}
