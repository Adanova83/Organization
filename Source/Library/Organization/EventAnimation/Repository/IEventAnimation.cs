namespace Library.Repository;

public interface IEventAnimation : IRepository<Domain.EventAnimation>
{
    Task<RepositoryResponseRange<Domain.EventAnimation>> SearchAsync(Guid? eventId, Guid? memberId, int page, int pageSize, CancellationToken ct);
}
