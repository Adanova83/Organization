using Library.Authentication;
using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Group : IGroup
{
    private readonly FullDbContext _dbContext;

    public Group(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Group>> CreateAsync(Domain.Group domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Groups.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Group>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.Group domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Groups.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Group domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Groups.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Group>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Group>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Group>> SearchByCodeAsync(string code, CancellationToken ct)
    {
        var entity = await _dbContext.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Code.ToUpper() == code.ToUpper(), ct);
        return new RepositoryResponse<Domain.Group>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Group>> SearchByNameAsync(string name, CancellationToken ct)
    {
        var entity = await _dbContext.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Name.ToUpper() == name.ToUpper(), ct);
        return new RepositoryResponse<Domain.Group>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
