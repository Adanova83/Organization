using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class OrganizationSession : IOrganizationSession
{
    private readonly FullDbContext _dbContext;

    public OrganizationSession(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Lectures avec l'année scolaire, qui porte les dates.
    private IQueryable<Entity.OrganizationSession> Query => _dbContext.OrganizationSessions.Include(item => item.SchoolYear);

    public async Task<RepositoryResponse<Domain.OrganizationSession>> CreateAsync(Domain.OrganizationSession domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.OrganizationSessions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        await _dbContext.Entry(entity).Reference(item => item.SchoolYear).LoadAsync(ct);
        return new RepositoryResponse<Domain.OrganizationSession>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.OrganizationSession domain, CancellationToken ct)
    {
        var entity = await _dbContext.OrganizationSessions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.OrganizationSessions.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.OrganizationSession domain, CancellationToken ct)
    {
        var entity = await _dbContext.OrganizationSessions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.OrganizationSession>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await Query
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.OrganizationSession>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public Task<RepositoryResponseRange<Domain.OrganizationSession>> SearchAsync(Guid OrganizationId, int page, int pageSize, CancellationToken ct)
        => ListAsync(Query.Where(item => item.OrganizationId == OrganizationId), page, pageSize, ct);

    public Task<RepositoryResponseRange<Domain.OrganizationSession>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct)
        => ListAsync(Query.Where(item => item.SchoolYearId == schoolYearId), page, pageSize, ct);

    private async Task<RepositoryResponseRange<Domain.OrganizationSession>> ListAsync(
        IQueryable<Entity.OrganizationSession> q, int page, int pageSize, CancellationToken ct)
    {
        // La plus récente d'abord.
        q = q.OrderByDescending(item => item.SchoolYear.StartDate);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.OrganizationSession>()
        {
            Data = entities
             .Select(Mapper.OrganizationSession.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.OrganizationSessions.CountAsync(ct)
        };
    }
}
