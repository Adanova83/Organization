namespace Library.Repository;

public interface IAccessEvent : IRepository<Domain.AccessEvent>
{
    Task<RepositoryResponseRange<Domain.AccessEvent>> SearchAsync(Guid badgeId, int page, int pageSize, CancellationToken ct);
}
