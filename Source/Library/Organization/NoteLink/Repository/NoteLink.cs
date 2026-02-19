using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class NoteLink : INoteLink
{
    private readonly FullDbContext _dbContext;

    public NoteLink(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.NoteLink>> CreateAsync(Domain.NoteLink domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.NoteLinks.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.NoteLink>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.NoteLink domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.NoteLinks.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.NoteLink domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.NoteLinks.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.NoteLink>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.NoteLinks
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.NoteLink>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
