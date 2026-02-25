using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Note : INote
{
    private readonly FullDbContext _dbContext;

    public Note(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Note>> CreateAsync(Domain.Note domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Notes.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Note>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Note domain, CancellationToken ct)
    {
        var entity = await _dbContext.Notes.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Notes.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Note domain, CancellationToken ct)
    {
        var entity = await _dbContext.Notes.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Note>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Notes
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Note>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
