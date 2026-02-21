namespace Library.Repository;

public interface IRole : IRepository<Domain.Role>
{
    Task<RepositoryResponse<Domain.Role>> SearchByCodeAsync(string type, string value, CancellationToken ct);
    Task<RepositoryResponse<Domain.Role>> SearchByNameAsync(string name, CancellationToken ct);
}
