using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Event : IEvent
{
    private readonly FullDbContext _dbContext;

    public Event(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Event>> CreateAsync(Domain.Event domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Events.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        await _dbContext.Entry(entity).Reference(item => item.SchoolYear).LoadAsync(ct);
        return new RepositoryResponse<Domain.Event>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Event domain, CancellationToken ct)
    {
        var entity = await _dbContext.Events.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Events.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Event domain, CancellationToken ct)
    {
        var entity = await _dbContext.Events.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Event>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Events.Include(item => item.SchoolYear)
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Event>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Event>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Event> q = _dbContext.Events.Include(item => item.SchoolYear);
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}%";
            q = q.Where(item =>
                EF.Functions.ILike(item.Code, pattern) ||
                EF.Functions.ILike(item.Name, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Name, "C"))
            .ThenBy(item => EF.Functions.Collate(item.Code, "C"));

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Event>()
        {
            Data = entities
             .Select(Mapper.Event.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Events.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponseRange<Domain.Event>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Event> q = _dbContext.Events.Include(item => item.SchoolYear)
            .Where(item => item.SchoolYearId == schoolYearId)
            .OrderBy(item => item.StartDate);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q.ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Event>()
        {
            Data = entities.Select(Mapper.Event.EntityToDomain).ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Events.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponseRange<Domain.Event>> SearchBySeriesAsync(Guid seriesId, CancellationToken ct)
    {
        var entities = await _dbContext.Events.Include(item => item.SchoolYear)
            .Where(item => item.SeriesId == seriesId)
            .OrderBy(item => item.StartDate)
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Event>()
        {
            Data = entities.Select(Mapper.Event.EntityToDomain).ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Events.CountAsync(ct)
        };
    }

    /// <summary>Crée plusieurs événements en un seul enregistrement (tout ou rien).</summary>
    public async Task<IReadOnlyList<Domain.Event>> CreateRangeAsync(IReadOnlyList<Domain.Event> domains, CancellationToken ct)
    {
        var entities = domains.Select(domain =>
        {
            var entity = domain.DomainToEntity();
            entity.Id = Guid.NewGuid();
            return entity;
        }).ToList();

        _dbContext.Events.AddRange(entities);
        await _dbContext.SaveChangesAsync(ct);

        foreach (var entity in entities)
        {
            await _dbContext.Entry(entity).Reference(item => item.SchoolYear).LoadAsync(ct);
        }

        return entities.Select(Mapper.Event.EntityToDomain).ToList();
    }
}
