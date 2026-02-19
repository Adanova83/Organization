namespace Library.Repository;

public interface IProjectSlot : IRepository<Domain.ProjectSlot>
{
    Task<RepositoryResponseRange<Domain.ProjectSlot>> SearchAsync(Guid projectId, int page, int pageSize, CancellationToken ct);
}
