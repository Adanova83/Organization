using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Contribution : IContribution
{
    private readonly FullDbContext _dbContext;

    public Contribution(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Contribution>> CreateAsync(Domain.Contribution domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Contributions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Contribution>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.Contribution domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Contributions.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Contribution domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Contributions.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Contribution>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Contributions
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Contribution>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Contribution>> SearchAsync(Guid memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Contribution> q = _dbContext.Contributions.AsNoTracking();

        q = q.Where(item => item.MemberId == memberId)
            .OrderBy(item => EF.Functions.Collate(item.OrganizationSessionId, "C"));

        if(page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .OrderBy(item => item.PayedDate)
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Contribution>()
        {
            Data = entities
             .Select(Mapper.Contribution.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Contributions.CountAsync(ct)
        };
    }
}
