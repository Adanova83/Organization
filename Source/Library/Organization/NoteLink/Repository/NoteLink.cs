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

    public async Task DeleteAsync(Domain.NoteLink domain, CancellationToken ct)
    {
        var entity = await _dbContext.NoteLinks.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.NoteLinks.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.NoteLink domain, CancellationToken ct)
    {
        var entity = await _dbContext.NoteLinks.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.NoteLink>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.NoteLinks
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.NoteLink>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
