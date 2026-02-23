using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class AccessEvent : IAccessEvent
{
    private readonly FullDbContext _dbContext;

    public AccessEvent(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.AccessEvent>> CreateAsync(Domain.AccessEvent domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = new Guid();
        _dbContext.AccessEvents.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.AccessEvent>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.AccessEvent domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.AccessEvents.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.AccessEvent domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.AccessEvents.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.AccessEvent>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.AccessEvents
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.AccessEvent>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.AccessEvent>> SearchAsync(Guid badgeId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.AccessEvent> q = _dbContext.AccessEvents;

        q = q.Where(item => item.BadgeId == badgeId)
            .OrderBy(item => EF.Functions.Collate(item.AccessedAt, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.AccessEvent>()
        {
            Data = entities
             .Select(Mapper.AccessEvent.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.AccessEvents.CountAsync(ct)
        };
    }
}
