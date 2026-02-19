using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class BadgeAssignation : IBadgeAssignation
{
    private readonly FullDbContext _dbContext;

    public BadgeAssignation(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.BadgeAssignation>> CreateAsync(Domain.BadgeAssignation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.BadgeAssignations.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.BadgeAssignation>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.BadgeAssignation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.BadgeAssignations.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.BadgeAssignation domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.BadgeAssignations.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.BadgeAssignation>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.BadgeAssignations
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.BadgeAssignation>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.BadgeAssignation>> SearchAsync(Guid? badgeId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.BadgeAssignation> q = _dbContext.BadgeAssignations.AsNoTracking();
        if (badgeId.HasValue)
        {
            q = q.Where(item => item.BadgeId.Equals(badgeId.Value));
        }

        if (memberId.HasValue)
        {
            q = q.Where(item => item.MemberId.Equals(memberId.Value));
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.BadgeId, "C"))
            .ThenBy(item => EF.Functions.Collate(item.MemberId, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.BadgeAssignation>()
        {
            Data = entities
             .Select(Mapper.BadgeAssignation.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.BadgeAssignations.CountAsync(ct)
        };
    }
}
