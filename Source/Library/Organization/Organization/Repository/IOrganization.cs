namespace Library.Repository;

public interface IOrganization : IRepository<Domain.Organization>
{
    Task<RepositoryResponseRange<Domain.Organization>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
