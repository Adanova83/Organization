using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ActivitySessionSlot : IActivitySessionSlot
{
    private readonly FullDbContext _dbContext;

    public ActivitySessionSlot(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ActivitySessionSlot>> CreateAsync(Domain.ActivitySessionSlot domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ActivitySessionSlots.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ActivitySessionSlot>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.ActivitySessionSlot domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ActivitySessionSlots.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.ActivitySessionSlot domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ActivitySessionSlots.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.ActivitySessionSlot>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessionSlots
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ActivitySessionSlot>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.ActivitySessionSlot>> SearchAsync(Guid sessionId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ActivitySessionSlot> q = _dbContext.ActivitySessionSlots.AsNoTracking();

        q = q.Where(item => item.SessionId.Equals(sessionId))
            .OrderBy(item => EF.Functions.Collate(item.DayOfWeek, "C"))
            .ThenBy(item => EF.Functions.Collate(item.StartTime, "C"))
            .ThenBy(item => EF.Functions.Collate(item.EndTime, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.ActivitySessionSlot>()
        {
            Data = entities
             .Select(Mapper.ActivitySessionSlot.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.ActivitySessionSlots.CountAsync(ct)
        };
    }
}
