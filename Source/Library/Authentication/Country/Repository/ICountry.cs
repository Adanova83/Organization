namespace Library.Repository;

public interface ICountry : IRepository<Domain.Country>
{
    Task<RepositoryResponseRange<Domain.Country>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
