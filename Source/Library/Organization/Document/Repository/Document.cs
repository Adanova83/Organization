using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Document : IDocument
{
    private readonly FullDbContext _dbContext;

    public Document(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Document>> CreateAsync(Domain.Document domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Documents.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Document>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Document domain, CancellationToken ct)
    {
        var entity = await _dbContext.Documents.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Documents.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Document domain, CancellationToken ct)
    {
        var entity = await _dbContext.Documents.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Document>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Documents
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Document>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Document>> SearchAsync(Guid memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Document> q = _dbContext.Documents;

        q = q.Where(item => item.MemberId == memberId)
            .OrderBy(item => EF.Functions.Collate(item.Title, "C"));

        if(page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Document>()
        {
            Data = entities
             .Select(Mapper.Document.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Documents.CountAsync(ct)
        };
    }
}
