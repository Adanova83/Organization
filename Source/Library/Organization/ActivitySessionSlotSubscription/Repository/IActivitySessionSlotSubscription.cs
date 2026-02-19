namespace Library.Repository;

public interface IActivitySessionSlotSubscription : IRepository<Domain.ActivitySessionSlotSubscription>
{
    Task<RepositoryResponseRange<Domain.ActivitySessionSlotSubscription>> SearchAsync(Guid? sessionSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct);
}
