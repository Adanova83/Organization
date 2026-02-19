namespace Library.Repository;

public interface IActivitySessionSlotAnimation : IRepository<Domain.ActivitySessionSlotAnimation>
{
    Task<RepositoryResponseRange<Domain.ActivitySessionSlotAnimation>> SearchAsync(Guid? sessionSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct);
}
