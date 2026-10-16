namespace Library.Repository;

public interface ISchoolYear : IRepository<Domain.SchoolYear>
{
    Task<RepositoryResponseRange<Domain.SchoolYear>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
