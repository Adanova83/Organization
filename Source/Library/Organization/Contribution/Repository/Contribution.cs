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

    public async Task DeleteAsync(Domain.Contribution domain, CancellationToken ct)
    {
        var entity = await _dbContext.Contributions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Contributions.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Contribution domain, CancellationToken ct)
    {
        var entity = await _dbContext.Contributions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Contribution>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Contributions
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Contribution>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Contribution>> SearchAsync(Guid memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Contribution> q = _dbContext.Contributions;

        q = q.Where(item => item.MemberId == memberId)
            .OrderBy(item => item.PayedDate);

        var filteredCount = await q.CountAsync(ct);

        if(page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Contribution>()
        {
            Data = entities
             .Select(Mapper.Contribution.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Contributions.CountAsync(ct)
        };
    }
}
