using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class EventSubscription : IEventSubscription
{
    private readonly FullDbContext _dbContext;

    public EventSubscription(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.EventSubscription>> CreateAsync(Domain.EventSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.EventSubscriptions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.EventSubscription>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.EventSubscription domain, CancellationToken ct)
    {
        var entity = await _dbContext.EventSubscriptions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.EventSubscriptions.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.EventSubscription domain, CancellationToken ct)
    {
        var entity = await _dbContext.EventSubscriptions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.EventSubscription>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.EventSubscriptions
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.EventSubscription>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.EventSubscription>> SearchAsync(Guid? eventId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.EventSubscription> q = _dbContext.EventSubscriptions;
        if (eventId.HasValue)
        {
            q = q.Where(item => item.EventId.Equals(eventId.Value));
        }

        if (memberId.HasValue)
        {
            q = q.Where(item => item.MemberId.Equals(memberId.Value));
        }

        q = q.OrderBy(item => item.EventId)
            .ThenBy(item => item.MemberId);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.EventSubscription>()
        {
            Data = entities
             .Select(Mapper.EventSubscription.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.EventSubscriptions.CountAsync(ct)
        };
    }
}
