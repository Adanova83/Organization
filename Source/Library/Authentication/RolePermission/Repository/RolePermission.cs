using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class GroupPermission : IRolePermission
{
    private readonly FullDbContext _dbContext;

    public GroupPermission(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.RolePermission>> CreateAsync(Domain.RolePermission domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.RolePermissions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.RolePermission>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.RolePermission domain, CancellationToken ct)
    {
        var entity = await _dbContext.RolePermissions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.RolePermissions.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.RolePermission domain, CancellationToken ct)
    {
        var entity = await _dbContext.RolePermissions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.RolePermission>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.RolePermissions
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.RolePermission>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.RolePermission>> SearchByPermissionIdAsync(Guid permissionId, CancellationToken ct)
    {
        IQueryable<Entity.RolePermission> q = _dbContext.RolePermissions;

        q = q.Where(item => item.PermissionId == permissionId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.RolePermission>()
        {
            Data = entities
             .Select(Mapper.RolePermission.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.RolePermissions.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponseRange<Domain.RolePermission>> SearchByRoleIdAsync(Guid roleId, CancellationToken ct)
    {
        IQueryable<Entity.RolePermission> q = _dbContext.RolePermissions;

        q = q.Where(item => item.RoleId == roleId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.RolePermission>()
        {
            Data = entities
             .Select(Mapper.RolePermission.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.RolePermissions.CountAsync(ct)
        };
    }
}
