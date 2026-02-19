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

    public Task DeleteAsync(Domain.ActivitySessionSlotAnimation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ActivitySessionSlotAnimations.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.ActivitySessionSlotAnimation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ActivitySessionSlotAnimations.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.ActivitySessionSlotAnimation>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessionSlotAnimations
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ActivitySessionSlotAnimation>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.ActivitySessionSlotAnimation>> SearchAsync(Guid? sessionSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ActivitySessionSlotAnimation> q = _dbContext.ActivitySessionSlotAnimations.AsNoTracking();
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

        return new RepositoryResponseRange<Domain.ActivitySessionSlotAnimation>()
        {
            Data = entities
             .Select(Mapper.ActivitySessionSlotAnimation.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.ActivitySessionSlotAnimations.CountAsync(ct)
        };
    }
}
