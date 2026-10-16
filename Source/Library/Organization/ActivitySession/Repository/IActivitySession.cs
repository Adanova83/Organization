namespace Library.Repository;

public interface IActivitySession : IRepository<Domain.ActivitySession>
{
    Task<RepositoryResponseRange<Domain.ActivitySession>> SearchAsync(Guid activityId, int page, int pageSize, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.ActivitySession>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct);
}
