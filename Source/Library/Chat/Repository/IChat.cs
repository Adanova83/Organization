namespace Library.Repository;

public interface IChat : IRepository<Domain.Chat>
{
    Task<RepositoryResponseRange<Domain.Chat>> SearchAsync(Guid memberId, DateOnly dateFrom, DateOnly dateTo, CancellationToken ct);
}
