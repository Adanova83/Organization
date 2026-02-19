namespace Library.Repository;

public interface ISpace : IRepository<Domain.Space>
{
    Task<RepositoryResponseRange<Domain.Space>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
