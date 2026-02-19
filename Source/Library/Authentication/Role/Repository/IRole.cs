namespace Library.Repository;

public interface IRole : IRepository<Domain.Role>
{
    Task<RepositoryResponseRange<Domain.Role>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
