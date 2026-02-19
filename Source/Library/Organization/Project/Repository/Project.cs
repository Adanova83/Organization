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
        return new RepositoryResponse<Domain.Project>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.Project domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Projects.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Project domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Projects.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Project>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Project>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Project>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Project> q = _dbContext.Projects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}";
            q = q.Where(item =>
                EF.Functions.ILike(item.Code, pattern) ||
                EF.Functions.ILike(item.Name, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Name, "C"))
            .ThenBy(item => EF.Functions.Collate(item.StartDate, "C"))
            .ThenBy(item => EF.Functions.Collate(item.EndDate, "C"));

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
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Projects.CountAsync(ct)
        };
    }
}
