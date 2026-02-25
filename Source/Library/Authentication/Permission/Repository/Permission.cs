using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Permission : IPermission
{
    private readonly FullDbContext _dbContext;

    public Permission(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Permission>> CreateAsync(Domain.Permission domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Permissions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Permission>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Permission domain, CancellationToken ct)
    {
        var entity = await _dbContext.Permissions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Permissions.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Permission domain, CancellationToken ct)
    {
        var entity = await _dbContext.Permissions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Permission>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Permissions
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Permission>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Permission>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Permission> q = _dbContext.Permissions;
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}";
            q = q.Where(item =>
                EF.Functions.ILike(item.Value, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Value, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Permission>()
        {
            Data = entities
             .Select(Mapper.Permission.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Members.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponse<Domain.Permission>> SearchByCodeAsync(string type, string value, CancellationToken ct)
    {
        var entity = await _dbContext.Permissions
            .FirstOrDefaultAsync(item => item.Type.ToUpper() == type.ToUpper() && item.Value.ToUpper() == value.ToUpper(), ct);
        return new RepositoryResponse<Domain.Permission>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Permission>> SearchByNameAsync(string name, CancellationToken ct)
    {
        var entity = await _dbContext.Permissions
            .FirstOrDefaultAsync(item => item.Value.ToUpper() == name.ToUpper(), ct);
        return new RepositoryResponse<Domain.Permission>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
