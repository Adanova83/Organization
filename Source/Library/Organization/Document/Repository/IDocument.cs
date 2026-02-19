namespace Library.Repository;

public interface IDocument : IRepository<Domain.Document>
{
    Task<RepositoryResponseRange<Domain.Document>> SearchAsync(Guid memberId, int page, int pageSize, CancellationToken ct);
}
