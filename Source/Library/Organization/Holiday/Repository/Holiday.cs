using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Holiday : IHoliday
{
    private readonly FullDbContext _dbContext;

    public Holiday(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Holiday>> CreateAsync(Domain.Holiday domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Holidays.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Holiday>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Holiday domain, CancellationToken ct)
    {
        var entity = await _dbContext.Holidays.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Holidays.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Holiday domain, CancellationToken ct)
    {
        var entity = await _dbContext.Holidays.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Holiday>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Holidays
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Holiday>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Holiday>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Holiday> q = _dbContext.Holidays;
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}";
            q = q.Where(item =>
                EF.Functions.ILike(item.Name, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.StartDate, "C"))
            .ThenBy(item => EF.Functions.Collate(item.EndDate, "C"))
            .ThenBy(item => EF.Functions.Collate(item.Name, "C"));

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Holiday>()
        {
            Data = entities
             .Select(Mapper.Holiday.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Holidays.CountAsync(ct)
        };
    }
}
