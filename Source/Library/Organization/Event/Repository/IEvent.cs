namespace Library.Repository;

public interface IEvent : IRepository<Domain.Event>
{
    Task<RepositoryResponseRange<Domain.Event>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
