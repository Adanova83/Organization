namespace Library.Repository;

public interface INoteLink : IRepository<Domain.NoteLink>
{
    Task<RepositoryResponseRange<Domain.NoteLink>> SearchAsync(Guid noteId, int page, int pageSize, CancellationToken ct);
}
