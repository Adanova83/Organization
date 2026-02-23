using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ActivitySession : IActivitySession
{
    private readonly FullDbContext _dbContext;

    public ActivitySession(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ActivitySession>> CreateAsync(Domain.ActivitySession domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ActivitySessions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ActivitySession>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.ActivitySession domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ActivitySessions.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.ActivitySession domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ActivitySessions.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.ActivitySession>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessions
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ActivitySession>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.ActivitySession>> SearchAsync(Guid activityId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ActivitySession> q = _dbContext.ActivitySessions;

        q = q.Where(item => item.ActivityId == activityId)
            .OrderBy(item => EF.Functions.Collate(item.StartDate, "C"))
            .ThenBy(item => EF.Functions.Collate(item.EndDate, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.ActivitySession>()
        {
            Data = entities
             .Select(Mapper.ActivitySession.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.ActivitySessions.CountAsync(ct)
        };
    }
}
