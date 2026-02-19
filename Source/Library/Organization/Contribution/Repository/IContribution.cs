namespace Library.Repository;

public interface IContribution : IRepository<Domain.Contribution>
{
    Task<RepositoryResponseRange<Domain.Contribution>> SearchAsync(Guid memberId, int page, int pageSize, CancellationToken ct);
}
