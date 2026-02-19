namespace Library.Repository;

public interface IOrganizationSession : IRepository<Domain.OrganizationSession>
{
    Task<RepositoryResponseRange<Domain.OrganizationSession>> SearchAsync(Guid OrganizationId, int page, int pageSize, CancellationToken ct);
}

