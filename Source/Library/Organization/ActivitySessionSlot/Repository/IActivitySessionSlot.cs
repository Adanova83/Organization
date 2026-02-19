namespace Library.Repository;

public interface IActivitySessionSlot : IRepository<Domain.ActivitySessionSlot>
{
    Task<RepositoryResponseRange<Domain.ActivitySessionSlot>> SearchAsync(Guid sessionId, int page, int pageSize, CancellationToken ct);
}
