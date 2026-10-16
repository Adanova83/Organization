using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Badge : IBadge
{
    private readonly FullDbContext _dbContext;

    public Badge(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Badge>> CreateAsync(Domain.Badge domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Badges.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Badge>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.Badge domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Badges.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Badge domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Badges.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Badge>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Badges
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Badge>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Badge>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Badge> q = _dbContext.Badges;
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}%";
            q = q.Where(item =>
                EF.Functions.ILike(item.Identifier, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Identifier, "C"));

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Badge>()
        {
            Data = entities
             .Select(Mapper.Badge.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Badges.CountAsync(ct)
        };
    }
}
