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
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
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

    public async Task<RepositoryResponseRange<Domain.NoteLink>> SearchAsync(Guid noteId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.NoteLink> q = _dbContext.NoteLinks
            .Where(item => item.NoteId == noteId)
            .OrderByDescending(item => item.Audit.LastUpdatedAt);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q.ToListAsync(ct);

        return new RepositoryResponseRange<Domain.NoteLink>()
        {
            Data = entities
             .Select(Mapper.NoteLink.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.NoteLinks.CountAsync(ct)
        };
    }
}
