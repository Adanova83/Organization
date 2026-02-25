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

    public async Task DeleteAsync(Domain.Role domain, CancellationToken ct)
    {
        var entity = await _dbContext.Roles.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Roles.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Role domain, CancellationToken ct)
    {
        var entity = await _dbContext.Roles.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Role>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Roles
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Role>> SearchByCodeAsync(string code, CancellationToken ct)
    {
        var entity = await _dbContext.Roles
            .FirstOrDefaultAsync(item => item.Code.ToUpper() == code.ToUpper(), ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Role>> SearchByNameAsync(string name, CancellationToken ct)
    {
        var entity = await _dbContext.Roles
            .FirstOrDefaultAsync(item => item.Name.ToUpper() == name.ToUpper(), ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
