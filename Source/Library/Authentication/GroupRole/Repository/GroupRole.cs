using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class GroupRole : IGroupRole
{
    private readonly FullDbContext _dbContext;

    public GroupRole(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.GroupRole>> CreateAsync(Domain.GroupRole domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.GroupRoles.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.GroupRole>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.GroupRole domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.GroupRoles.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.GroupRole domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.GroupRoles.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.GroupRole>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.GroupRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.GroupRole>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.GroupRole>> SearchByRoleIdAsync(Guid RoleId, CancellationToken ct)
    {
        IQueryable<Entity.GroupRole> q = _dbContext.GroupRoles.AsNoTracking();

        q = q.Where(item => item.RoleId == RoleId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.GroupRole>()
        {
            Data = entities
             .Select(Mapper.GroupRole.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Roles.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponseRange<Domain.GroupRole>> SearchByGroupIdAsync(Guid GroupId, CancellationToken ct)
    {
        IQueryable<Entity.GroupRole> q = _dbContext.GroupRoles.AsNoTracking();

        q = q.Where(item => item.GroupId == GroupId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.GroupRole>()
        {
            Data = entities
             .Select(Mapper.GroupRole.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Roles.CountAsync(ct)
        };
    }
}
