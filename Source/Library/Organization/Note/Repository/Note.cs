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

    public Task DeleteAsync(Domain.Note domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Notes.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Note domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Notes.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Note>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Note>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
