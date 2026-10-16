using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Project : IProject
{
    private readonly FullDbContext _dbContext;

    public Project(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Project>> CreateAsync(Domain.Project domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Projects.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        await _dbContext.Entry(entity).Reference(item => item.SchoolYear).LoadAsync(ct);
        return new RepositoryResponse<Domain.Project>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Project domain, CancellationToken ct)
    {
        var entity = await _dbContext.Projects.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Projects.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Project domain, CancellationToken ct)
    {
        var entity = await _dbContext.Projects.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Project>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Projects.Include(item => item.SchoolYear)
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Project>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Project>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Project> q = _dbContext.Projects.Include(item => item.SchoolYear);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}%";
            q = q.Where(item =>
                EF.Functions.ILike(item.Code, pattern) ||
                EF.Functions.ILike(item.Name, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Name, "C"))
            .ThenBy(item => item.StartDate)
            .ThenBy(item => item.EndDate);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Project>()
        {
            Data = entities
             .Select(Mapper.Project.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Projects.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponseRange<Domain.Project>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Project> q = _dbContext.Projects.Include(item => item.SchoolYear)
            .Where(item => item.SchoolYearId == schoolYearId)
            .OrderBy(item => EF.Functions.Collate(item.Name, "C"));

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q.ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Project>()
        {
            Data = entities.Select(Mapper.Project.EntityToDomain).ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Projects.CountAsync(ct)
        };
    }
}
