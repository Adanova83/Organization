namespace Library.Repository;

public interface IProject : IRepository<Domain.Project>
{
    Task<RepositoryResponseRange<Domain.Project>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.Project>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct);
}
