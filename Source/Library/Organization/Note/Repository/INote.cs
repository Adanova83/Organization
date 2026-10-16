namespace Library.Repository;

public interface INote : IRepository<Domain.Note>
{
    Task<RepositoryResponseRange<Domain.Note>> SearchAsync(Guid memberId, int page, int pageSize, CancellationToken ct);
}
