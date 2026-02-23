using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ProjectSlotAnimation : IProjectSlotAnimation
{
    private readonly FullDbContext _dbContext;

    public ProjectSlotAnimation(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ProjectSlotAnimation>> CreateAsync(Domain.ProjectSlotAnimation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ProjectSlotAnimations.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.ProjectSlotAnimation>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.ProjectSlotAnimation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ProjectSlotAnimations.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.ProjectSlotAnimation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.ProjectSlotAnimations.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.ProjectSlotAnimation>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ProjectSlotAnimations
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.ProjectSlotAnimation>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.ProjectSlotAnimation>> SearchAsync(Guid? projectSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ProjectSlotAnimation> q = _dbContext.ProjectSlotAnimations;
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

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.ProjectSlotAnimation>()
        {
            Data = entities
             .Select(Mapper.ProjectSlotAnimation.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.ProjectSlotAnimations.CountAsync(ct)
        };
    }
}
