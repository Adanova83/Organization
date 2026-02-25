using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class RoleMember : IRoleMember
{
    private readonly FullDbContext _dbContext;

    public RoleMember(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.RoleMember>> CreateAsync(Domain.RoleMember domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.RoleMembers.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.RoleMember>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.RoleMember domain, CancellationToken ct)
    {
        var entity = await _dbContext.RoleMembers.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.RoleMembers.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.RoleMember domain, CancellationToken ct)
    {
        var entity = await _dbContext.RoleMembers.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.RoleMember>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.RoleMembers
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.RoleMember>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.RoleMember>> SearchByMemberIdAsync(Guid memberId, CancellationToken ct)
    {
        IQueryable<Entity.RoleMember> q = _dbContext.RoleMembers;

        q = q.Where(item => item.MemberId == memberId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.RoleMember>()
        {
            Data = entities
             .Select(Mapper.RoleMember.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Members.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponseRange<Domain.RoleMember>> SearchByRoleIdAsync(Guid GroupId, CancellationToken ct)
    {
        IQueryable<Entity.RoleMember> q = _dbContext.RoleMembers;

        q = q.Where(item => item.RoleId == GroupId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.RoleMember>()
        {
            Data = entities
             .Select(Mapper.RoleMember.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Members.CountAsync(ct)
        };
    }
}
