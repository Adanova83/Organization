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
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
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
            var pattern = $"%{query}%";
            q = q.Where(item =>
                EF.Functions.ILike(item.Name, pattern)
            );
        }

        q = q.OrderBy(item => item.StartDate)
            .ThenBy(item => item.EndDate)
            .ThenBy(item => EF.Functions.Collate(item.Name, "C"));

        var filteredCount = await q.CountAsync(ct);

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
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Holidays.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponseRange<Domain.Holiday>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Holiday> q = _dbContext.Holidays
            .Where(item => item.SchoolYearId == schoolYearId)
            .OrderBy(item => item.StartDate);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q.ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Holiday>()
        {
            Data = entities.Select(Mapper.Holiday.EntityToDomain).ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Holidays.CountAsync(ct)
        };
    }
}
