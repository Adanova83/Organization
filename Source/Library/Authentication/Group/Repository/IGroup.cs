namespace Library.Repository;

public  interface IGroup : IRepository<Domain.Group>
{
    Task<RepositoryResponse<Domain.Group>> SearchByCodeAsync(string code, CancellationToken ct);
    Task<RepositoryResponse<Domain.Group>> SearchByNameAsync(string name, CancellationToken ct);
}
