namespace Library.Repository;

public interface IChatMessage : IRepository<Domain.ChatMessage>
{
    Task<RepositoryResponseRange<Domain.ChatMessage>> SearchAsync(Guid memberId, DateOnly dateFrom, DateOnly dateTo, CancellationToken ct);
}
