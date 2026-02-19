using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Space : ISpace
{
    private readonly FullDbContext _dbContext;

    public Space(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Space>> CreateAsync(Domain.Space domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        entity = _dbContext.Spaces.Add(entity).Entity;
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Space>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.Space domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Spaces.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Space domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Spaces.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Space>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Space>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Space>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Space> q = _dbContext.Spaces.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}";
            q = q.Where(item =>
                EF.Functions.ILike(item.Name, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Name, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Space>()
        {
            Data = entities
             .Select(Mapper.Space.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Spaces.CountAsync(ct)
        };
    }
}
