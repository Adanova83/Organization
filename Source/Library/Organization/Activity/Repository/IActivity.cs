namespace Library.Repository;

public interface IActivity : IRepository<Domain.Activity>
{
    Task<RepositoryResponseRange<Domain.Activity>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
