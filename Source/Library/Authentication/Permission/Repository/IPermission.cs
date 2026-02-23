namespace Library.Repository;

public interface IPermission : IRepository<Domain.Permission>
{
    Task<RepositoryResponse<Domain.Permission>> SearchByCodeAsync(string type, string value, CancellationToken ct);
    Task<RepositoryResponse<Domain.Permission>> SearchByNameAsync(string name, CancellationToken ct);
}
