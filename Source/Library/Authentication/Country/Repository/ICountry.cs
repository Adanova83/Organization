namespace Library.Repository;

public interface ICountry : IRepository<Domain.Country>
{
    Task<RepositoryResponse<Domain.Country>> SearchByCodeAlpaha2Async(string codeAlpha2, CancellationToken ct);
    Task<RepositoryResponse<Domain.Country>> SearchByCodeAlpaha3Async(string codeAlpha3, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.Country>> SearchByNameAsync(string? query, int page, int pageSize, CancellationToken ct);
}
