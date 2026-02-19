namespace Library.Repository;

public interface IBadge : IRepository<Domain.Badge>
{
    Task<RepositoryResponseRange<Domain.Badge>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
