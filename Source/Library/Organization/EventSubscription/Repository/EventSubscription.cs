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

    public Task DeleteAsync(Domain.EventSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.EventSubscriptions.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.EventSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.EventSubscriptions.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
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

        q = q.OrderBy(item => EF.Functions.Collate(item.EventId, "C"))
            .ThenBy(item => EF.Functions.Collate(item.MemberId, "C"));

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
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.EventSubscriptions.CountAsync(ct)
        };
    }
}
