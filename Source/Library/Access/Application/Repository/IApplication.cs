namespace Library.Repository;

public interface IApplication : IRepository<Domain.Application>
{
    Task<RepositoryResponseRange<Domain.Application>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
