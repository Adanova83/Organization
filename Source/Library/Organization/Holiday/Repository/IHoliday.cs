namespace Library.Repository;

public interface IHoliday : IRepository<Domain.Holiday>
{
    Task<RepositoryResponseRange<Domain.Holiday>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
}
