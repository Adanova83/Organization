using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ProjectSlotSubscription : IProjectSlotSubscription
{
    private readonly FullDbContext _dbContext;

    public ProjectSlotSubscription(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ProjectSlotSubscription>> CreateAsync(Domain.ProjectSlotSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ProjectSlotSubscriptions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        var created = entity.EntityToDomain();
        await WithRanksAsync([created], ct);
        return new RepositoryResponse<Domain.ProjectSlotSubscription>()
        {
            Data = created
        };
    }

    public async Task DeleteAsync(Domain.ProjectSlotSubscription domain, CancellationToken ct)
    {
        var entity = await _dbContext.ProjectSlotSubscriptions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.ProjectSlotSubscriptions.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.ProjectSlotSubscription domain, CancellationToken ct)
    {
        var entity = await _dbContext.ProjectSlotSubscriptions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.ProjectSlotSubscription>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ProjectSlotSubscriptions
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        var found = entity?.EntityToDomain();
        if (found is not null)
        {
            await WithRanksAsync([found], ct);
        }

        return new RepositoryResponse<Domain.ProjectSlotSubscription>()
        {
            Data = found
        };
    }

    public async Task<RepositoryResponseRange<Domain.ProjectSlotSubscription>> SearchAsync(Guid? projectSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ProjectSlotSubscription> q = _dbContext.ProjectSlotSubscriptions;
        if (projectSlotId.HasValue)
        {
            q = q.Where(item => item.ProjectSlotId.Equals(projectSlotId.Value));
        }

        if (memberId.HasValue)
        {
            q = q.Where(item => item.MemberId.Equals(memberId.Value));
        }

        q = q.OrderBy(item => item.ProjectSlotId)
            .ThenBy(item => item.IsWaiting)
            .ThenBy(item => item.Audit.CreatedAt);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var data = entities.Select(Mapper.ProjectSlotSubscription.EntityToDomain).ToList();
        await WithRanksAsync(data, ct);

        return new RepositoryResponseRange<Domain.ProjectSlotSubscription>()
        {
            Data = data,
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.ProjectSlotSubscriptions.CountAsync(ct)
        };
    }

    /// <summary>Inscrits confirmés et en attente, par élément (sans les noms).</summary>
    public async Task<IReadOnlyDictionary<Guid, (int Confirmed, int Waiting)>> CountByTargetAsync(IReadOnlyCollection<Guid> targetIds, CancellationToken ct)
    {
        var rows = await _dbContext.ProjectSlotSubscriptions
            .Where(item => targetIds.Contains(item.ProjectSlotId))
            .GroupBy(item => new { item.ProjectSlotId, item.IsWaiting })
            .Select(group => new { Target = group.Key.ProjectSlotId, group.Key.IsWaiting, Count = group.Count() })
            .ToListAsync(ct);

        return rows.GroupBy(row => row.Target).ToDictionary(
            group => group.Key,
            group => (group.Where(row => !row.IsWaiting).Sum(row => row.Count), group.Where(row => row.IsWaiting).Sum(row => row.Count)));
    }

    /// <summary>Le premier de la liste d'attente (le plus ancien) devient inscrit ; null si la liste est vide.</summary>
    public async Task<Domain.ProjectSlotSubscription?> PromoteNextAsync(Guid targetId, CancellationToken ct)
    {
        var next = await _dbContext.ProjectSlotSubscriptions
            .Where(item => item.ProjectSlotId == targetId && item.IsWaiting)
            .OrderBy(item => item.Audit.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (next is null)
        {
            return null;
        }

        next.IsWaiting = false;
        await _dbContext.SaveChangesAsync(ct);
        return next.EntityToDomain();
    }

    /// <summary>Rang dans la liste d'attente (1 = le prochain à passer), par ordre d'inscription.</summary>
    private async Task WithRanksAsync(List<Domain.ProjectSlotSubscription> items, CancellationToken ct)
    {
        var targets = items.Where(item => item.IsWaiting).Select(item => item.ProjectSlotId).Distinct().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var waiting = await _dbContext.ProjectSlotSubscriptions
            .Where(item => item.IsWaiting && targets.Contains(item.ProjectSlotId))
            .OrderBy(item => item.Audit.CreatedAt)
            .Select(item => new { item.Id, Target = item.ProjectSlotId })
            .ToListAsync(ct);

        var ranks = waiting.GroupBy(item => item.Target)
            .SelectMany(group => group.Select((item, index) => (item.Id, Rank: index + 1)))
            .ToDictionary(item => item.Id, item => item.Rank);

        foreach (var item in items.Where(item => item.IsWaiting))
        {
            item.WaitingRank = ranks.GetValueOrDefault(item.Id);
        }
    }
}
