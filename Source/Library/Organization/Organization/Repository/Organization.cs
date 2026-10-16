using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Organization : IOrganization
{
    private readonly FullDbContext _dbContext;

    public Organization(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Organization>> CreateAsync(Domain.Organization domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Organizations.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Organization>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Organization domain, CancellationToken ct)
    {
        var entity = await _dbContext.Organizations.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Organizations.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Organization domain, CancellationToken ct)
    {
        var entity = await _dbContext.Organizations.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Organization>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Organizations
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Organization>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Organization>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Organization> q = _dbContext.Organizations;
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}%";
            q = q.Where(item =>
                EF.Functions.ILike(item.Name, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Name, "C"));

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Organization>()
        {
            Data = entities
             .Select(Mapper.Organization.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Organizations.CountAsync(ct)
        };
    }
}
