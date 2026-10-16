namespace Library.Repository;

public interface IEvent : IRepository<Domain.Event>
{
    Task<RepositoryResponseRange<Domain.Event>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.Event>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.Event>> SearchBySeriesAsync(Guid seriesId, CancellationToken ct);
    Task<IReadOnlyList<Domain.Event>> CreateRangeAsync(IReadOnlyList<Domain.Event> domains, CancellationToken ct);
}
