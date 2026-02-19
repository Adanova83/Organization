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

    public async Task<RepositoryResponse<Domain.OrganizationSession>> CreateAsync(Domain.OrganizationSession domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.OrganizationSessions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.OrganizationSession>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.OrganizationSession domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.OrganizationSessions.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.OrganizationSession domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.OrganizationSessions.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.OrganizationSession>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.OrganizationSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.OrganizationSession>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.OrganizationSession>> SearchAsync(Guid OrganizationId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.OrganizationSession> q = _dbContext.OrganizationSessions.AsNoTracking();

        q = q.Where(item => item.OrganizationId == OrganizationId)
            .OrderBy(item => EF.Functions.Collate(item.StartDate, "C"))
            .ThenBy(item => EF.Functions.Collate(item.EndDate, "C"));

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
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.OrganizationSessions.CountAsync(ct)
        };
    }
}
