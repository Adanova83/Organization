using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class PermissionMember : IPermissionMember
{
    private readonly FullDbContext _dbContext;

    public PermissionMember(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.PermissionMember>> CreateAsync(Domain.PermissionMember domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.PermissionMembers.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.PermissionMember>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.PermissionMember domain, CancellationToken ct)
    {
        var entity = await _dbContext.PermissionMembers.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.PermissionMembers.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.PermissionMember domain, CancellationToken ct)
    {
        var entity = await _dbContext.PermissionMembers.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.PermissionMember>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.PermissionMembers
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.PermissionMember>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.PermissionMember>> SearchByMemberIdAsync(Guid memberId, CancellationToken ct)
    {
        IQueryable<Entity.PermissionMember> q = _dbContext.PermissionMembers;

        q = q.Where(item => item.MemberId == memberId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.PermissionMember>()
        {
            Data = entities
             .Select(Mapper.PermissionMember.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Members.CountAsync(ct)
        };
    }
    
    public async Task<RepositoryResponseRange<Domain.PermissionMember>> SearchByPermissionIdAsync(Guid permissionId, CancellationToken ct)
    {
        IQueryable<Entity.PermissionMember> q = _dbContext.PermissionMembers;

        q = q.Where(item => item.PermissionId == permissionId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.PermissionMember>()
        {
            Data = entities
             .Select(Mapper.PermissionMember.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Members.CountAsync(ct)
        };
    }
}
