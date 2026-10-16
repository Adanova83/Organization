using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ActivitySessionSlotAnimation : IActivitySessionSlotAnimation
{
    private readonly FullDbContext _dbContext;

    public ActivitySessionSlotAnimation(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ActivitySessionSlotAnimation>> CreateAsync(Domain.ActivitySessionSlotAnimation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ActivitySessionSlotAnimations.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ActivitySessionSlotAnimation>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.ActivitySessionSlotAnimation domain, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessionSlotAnimations.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.ActivitySessionSlotAnimations.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.ActivitySessionSlotAnimation domain, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessionSlotAnimations.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.ActivitySessionSlotAnimation>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessionSlotAnimations
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ActivitySessionSlotAnimation>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.ActivitySessionSlotAnimation>> SearchAsync(Guid? sessionSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ActivitySessionSlotAnimation> q = _dbContext.ActivitySessionSlotAnimations;
        if (sessionSlotId.HasValue)
        {
            q = q.Where(item => item.SessionSlotId.Equals(sessionSlotId.Value));
        }

        if (memberId.HasValue)
        {
            q = q.Where(item => item.MemberId.Equals(memberId.Value));
        }

        q = q.OrderBy(item => item.SessionSlotId)
            .ThenBy(item => item.MemberId);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.ActivitySessionSlotAnimation>()
        {
            Data = entities
             .Select(Mapper.ActivitySessionSlotAnimation.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.ActivitySessionSlotAnimations.CountAsync(ct)
        };
    }
}
