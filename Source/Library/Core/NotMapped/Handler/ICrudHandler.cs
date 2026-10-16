using Library.Repository;

namespace Library.Handler;

/// <summary>Opérations communes des handlers, utilisées par les endpoints CRUD de l'API.</summary>
public interface ICrudHandler<TDomain>
{
    Task<RepositoryResponse<TDomain>> CreateAsync(TDomain domain, CancellationToken ct);
    Task EditAsync(TDomain domain, CancellationToken ct);
    Task DeleteAsync(TDomain domain, CancellationToken ct);
    Task<RepositoryResponse<TDomain>> SearchAsync(Guid id, CancellationToken ct);
}
