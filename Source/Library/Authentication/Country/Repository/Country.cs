using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Country : ICountry
{
    private readonly FullDbContext _dbContext;

    public Country(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Country>> CreateAsync(Domain.Country domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Countries.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Country>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public Task DeleteAsync(Domain.Country domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Countries.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Country domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Countries.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<RepositoryResponse<Domain.Country>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Country>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Country>> SearchByCodeAlpaha2Async(string codeAlpha2, CancellationToken ct)
    {
        var entity = await _dbContext.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.CodeAlpha2.ToUpper() == codeAlpha2.ToUpper(), ct);
        return new RepositoryResponse<Domain.Country>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Country>> SearchByCodeAlpaha3Async(string codeAlpha3, CancellationToken ct)
    {
        var entity = await _dbContext.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.CodeAlpha3.ToUpper() == codeAlpha3.ToUpper(), ct);
        return new RepositoryResponse<Domain.Country>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Country>> SearchByNameAsync(string? namePattern, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Country> q = _dbContext.Countries.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(namePattern))
        {
            var pattern = $"%{namePattern}";
            q = q.Where(item =>
                EF.Functions.ILike(item.Name, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Name, "C"))
            .ThenBy(item => EF.Functions.Collate(item.CodeAlpha3, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Country>()
        {
            Data = entities
             .Select(Mapper.Country.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Countries.CountAsync(ct)
        };
    }
}
