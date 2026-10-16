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

    // Lectures avec l'année scolaire, qui porte les dates.
    private IQueryable<Entity.ActivitySession> Query => _dbContext.ActivitySessions.Include(item => item.SchoolYear);

    public async Task<RepositoryResponse<Domain.ActivitySession>> CreateAsync(Domain.ActivitySession domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ActivitySessions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        await _dbContext.Entry(entity).Reference(item => item.SchoolYear).LoadAsync(ct);
        return new RepositoryResponse<Domain.ActivitySession>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.ActivitySession domain, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.ActivitySessions.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.ActivitySession domain, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.ActivitySession>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await Query
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ActivitySession>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public Task<RepositoryResponseRange<Domain.ActivitySession>> SearchAsync(Guid activityId, int page, int pageSize, CancellationToken ct)
        => ListAsync(Query.Where(item => item.ActivityId == activityId), page, pageSize, ct);

    public Task<RepositoryResponseRange<Domain.ActivitySession>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct)
        => ListAsync(Query.Where(item => item.SchoolYearId == schoolYearId), page, pageSize, ct);

    private async Task<RepositoryResponseRange<Domain.ActivitySession>> ListAsync(
        IQueryable<Entity.ActivitySession> q, int page, int pageSize, CancellationToken ct)
    {
        // Année la plus récente d'abord, puis par date de début propre (les sessions sur toute l'année en premier).
        q = q.OrderByDescending(item => item.SchoolYear.StartDate)
            .ThenBy(item => item.StartDate);

        var filteredCount = await q.CountAsync(ct);

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
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.ActivitySessions.CountAsync(ct)
        };
    }
}
