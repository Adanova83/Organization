using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ProjectSlot : IProjectSlot
{
    private readonly FullDbContext _dbContext;

    public ProjectSlot(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ProjectSlot>> CreateAsync(Domain.ProjectSlot domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ProjectSlots.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ProjectSlot>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.ProjectSlot domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ProjectSlots.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.ProjectSlot domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ProjectSlots.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.ProjectSlot>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ProjectSlots
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ProjectSlot>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.ProjectSlot>> SearchAsync(Guid projectId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ProjectSlot> q = _dbContext.ProjectSlots.AsNoTracking();

        q = q.Where(item => item.ProjectId.Equals(projectId))
            .OrderBy(item => EF.Functions.Collate(item.DayOfWeek, "C"))
            .ThenBy(item => EF.Functions.Collate(item.StartTime, "C"))
            .ThenBy(item => EF.Functions.Collate(item.EndTime, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.ProjectSlot>()
        {
            Data = entities
             .Select(Mapper.ProjectSlot.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.ProjectSlots.CountAsync(ct)
        };
    }
}
