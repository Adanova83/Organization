using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class SchoolYear : ISchoolYear
{
    private readonly FullDbContext _dbContext;

    public SchoolYear(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.SchoolYear>> CreateAsync(Domain.SchoolYear domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.SchoolYears.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.SchoolYear>()
        {
            Data = entity.EntityToDomain()
        };
    }

    /// <summary>
    /// Suppression logique, refusée tant que l'année est utilisée : les saisons, sessions, événements et projets qui s'y rattachent
    /// prennent leurs dates dans l'année et disparaîtraient avec elle.
    /// </summary>
    public async Task DeleteAsync(Domain.SchoolYear domain, CancellationToken ct)
    {
        var entity = await _dbContext.SchoolYears.FindAsync(domain.Id);
        if (entity == null)
        {
            return;
        }

        var inUse = await _dbContext.OrganizationSessions.AnyAsync(item => item.SchoolYearId == domain.Id, ct)
            || await _dbContext.ActivitySessions.AnyAsync(item => item.SchoolYearId == domain.Id, ct)
            || await _dbContext.Events.AnyAsync(item => item.SchoolYearId == domain.Id, ct)
            || await _dbContext.Projects.AnyAsync(item => item.SchoolYearId == domain.Id, ct)
            || await _dbContext.Holidays.AnyAsync(item => item.SchoolYearId == domain.Id, ct);

        if (inUse)
        {
            throw new InUseException("The school year is still used by seasons, sessions, events, projects or holidays.");
        }

        _dbContext.SchoolYears.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task EditAsync(Domain.SchoolYear domain, CancellationToken ct)
    {
        var entity = await _dbContext.SchoolYears.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.SchoolYear>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.SchoolYears
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.SchoolYear>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.SchoolYear>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.SchoolYear> q = _dbContext.SchoolYears;
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}%";
            q = q.Where(item => EF.Functions.ILike(item.Code, pattern));
        }

        // La plus récente d'abord.
        q = q.OrderByDescending(item => item.StartDate);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.SchoolYear>()
        {
            Data = entities
             .Select(Mapper.SchoolYear.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.SchoolYears.CountAsync(ct)
        };
    }
}
