using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ActivitySessionSlotSubscription : IActivitySessionSlotSubscription
{
    private readonly FullDbContext _dbContext;

    public ActivitySessionSlotSubscription(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ActivitySessionSlotSubscription>> CreateAsync(Domain.ActivitySessionSlotSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ActivitySessionSlotSubscriptions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ActivitySessionSlotSubscription>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.ActivitySessionSlotSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ActivitySessionSlotSubscriptions.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.ActivitySessionSlotSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ActivitySessionSlotSubscriptions.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.ActivitySessionSlotSubscription>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessionSlotSubscriptions
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ActivitySessionSlotSubscription>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.ActivitySessionSlotSubscription>> SearchAsync(Guid? sessionSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ActivitySessionSlotSubscription> q = _dbContext.ActivitySessionSlotSubscriptions;
        if (sessionSlotId.HasValue)
        {
            q = q.Where(item => item.SessionSlotId.Equals(sessionSlotId.Value));
        }

        if (memberId.HasValue)
        {
            q = q.Where(item => item.MemberId.Equals(memberId.Value));
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.SessionSlotId, "C"))
            .ThenBy(item => EF.Functions.Collate(item.MemberId, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.ActivitySessionSlotSubscription>()
        {
            Data = entities
             .Select(Mapper.ActivitySessionSlotSubscription.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.ActivitySessionSlotSubscriptions.CountAsync(ct)
        };
    }
}
