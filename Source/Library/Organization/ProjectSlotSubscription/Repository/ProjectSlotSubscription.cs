using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ProjectSlotSubscription : IProjectSlotSubscription
{
    private readonly FullDbContext _dbContext;

    public ProjectSlotSubscription(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ProjectSlotSubscription>> CreateAsync(Domain.ProjectSlotSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ProjectSlotSubscriptions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ProjectSlotSubscription>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.ProjectSlotSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ProjectSlotSubscriptions.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.ProjectSlotSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ProjectSlotSubscriptions.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.ProjectSlotSubscription>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ProjectSlotSubscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ProjectSlotSubscription>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.ProjectSlotSubscription>> SearchAsync(Guid? projectSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ProjectSlotSubscription> q = _dbContext.ProjectSlotSubscriptions.AsNoTracking();
        if (projectSlotId.HasValue)
        {
            q = q.Where(item => item.ProjectSlotId.Equals(projectSlotId.Value));
        }

        if (memberId.HasValue)
        {
            q = q.Where(item => item.MemberId.Equals(memberId.Value));
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.ProjectSlotId, "C"))
            .ThenBy(item => EF.Functions.Collate(item.MemberId, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.ProjectSlotSubscription>()
        {
            Data = entities
             .Select(Mapper.ProjectSlotSubscription.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.ProjectSlotSubscriptions.CountAsync(ct)
        };
    }
}
