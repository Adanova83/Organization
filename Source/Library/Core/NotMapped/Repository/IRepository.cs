namespace Library.Repository;

public interface IRepository<DomainType>
{
    Task<RepositoryResponse<DomainType>> CreateAsync(DomainType domain, CancellationToken ct);
    Task DeleteAsync(DomainType domain, CancellationToken ct);
    Task EditAsync(DomainType domain, CancellationToken ct);
    Task<RepositoryResponse<DomainType>> SearchAsync(Guid id, CancellationToken ct);
}
