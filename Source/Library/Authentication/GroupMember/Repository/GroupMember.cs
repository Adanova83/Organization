using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class GroupMember : IGroupMember
{
    private readonly FullDbContext _dbContext;

    public GroupMember(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.GroupMember>> CreateAsync(Domain.GroupMember domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.GroupMembers.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.GroupMember>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.GroupMember domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.GroupMembers.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.GroupMember domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.GroupMembers.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.GroupMember>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.GroupMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.GroupMember>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.GroupMember>> SearchByMemberIdAsync(Guid memberId, CancellationToken ct)
    {
        IQueryable<Entity.GroupMember> q = _dbContext.GroupMembers.AsNoTracking();

        q = q.Where(item => item.MemberId == memberId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.GroupMember>()
        {
            Data = entities
             .Select(Mapper.GroupMember.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Members.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponseRange<Domain.GroupMember>> SearchByGroupIdAsync(Guid GroupId, CancellationToken ct)
    {
        IQueryable<Entity.GroupMember> q = _dbContext.GroupMembers.AsNoTracking();

        q = q.Where(item => item.GroupId == GroupId);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.GroupMember>()
        {
            Data = entities
             .Select(Mapper.GroupMember.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Members.CountAsync(ct)
        };
    }
}
