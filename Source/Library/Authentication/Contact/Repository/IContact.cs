namespace Library.Repository;

public interface IContact : IRepository<Domain.Contact>
{
    Task<RepositoryResponseRange<Domain.Contact>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
