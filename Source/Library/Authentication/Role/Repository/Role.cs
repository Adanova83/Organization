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
        _dbContext.Groups.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.Role domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Groups.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Role domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Groups.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Role>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Groups
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Role>> SearchByCodeAsync(string code, CancellationToken ct)
    {
        var entity = await _dbContext.Groups
            .FirstOrDefaultAsync(item => item.Code.ToUpper() == code.ToUpper(), ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Role>> SearchByNameAsync(string name, CancellationToken ct)
    {
        var entity = await _dbContext.Groups
            .FirstOrDefaultAsync(item => item.Name.ToUpper() == name.ToUpper(), ct);
        return new RepositoryResponse<Domain.Role>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
