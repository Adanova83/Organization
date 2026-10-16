using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class EventAnimation : IEventAnimation
{
    private readonly FullDbContext _dbContext;

    public EventAnimation(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.EventAnimation>> CreateAsync(Domain.EventAnimation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.EventAnimations.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.EventAnimation>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.EventAnimation domain, CancellationToken ct)
    {
        var entity = await _dbContext.EventAnimations.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.EventAnimations.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.EventAnimation domain, CancellationToken ct)
    {
        var entity = await _dbContext.EventAnimations.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.EventAnimation>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.EventAnimations
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.EventAnimation>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.EventAnimation>> SearchAsync(Guid? eventId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.EventAnimation> q = _dbContext.EventAnimations;
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

        return new RepositoryResponseRange<Domain.EventAnimation>()
        {
            Data = entities
             .Select(Mapper.EventAnimation.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.EventAnimations.CountAsync(ct)
        };
    }
}
