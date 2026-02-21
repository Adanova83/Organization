using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Role : IRole
{
    private readonly FullDbContext _dbContext;

    public Role(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Role>> CreateAsync(Domain.Role domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Roles.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.Role domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Roles.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Role domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Roles.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Role>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Role>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Role> q = _dbContext.Roles.AsNoTracking();
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

        return new RepositoryResponseRange<Domain.Role>()
        {
            Data = entities
             .Select(Mapper.Role.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Members.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponse<Domain.Role>> SearchByCodeAsync(string type, string value, CancellationToken ct)
    {
        var entity = await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Type.ToUpper() == type.ToUpper() && item.Value.ToUpper() == value.ToUpper(), ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Role>> SearchByNameAsync(string name, CancellationToken ct)
    {
        var entity = await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Value.ToUpper() == name.ToUpper(), ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
