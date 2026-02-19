namespace Library.Repository;

public interface IProjectSlotAnimation : IRepository<Domain.ProjectSlotAnimation>
{
    Task<RepositoryResponseRange<Domain.ProjectSlotAnimation>> SearchAsync(Guid? projectSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct);
}
