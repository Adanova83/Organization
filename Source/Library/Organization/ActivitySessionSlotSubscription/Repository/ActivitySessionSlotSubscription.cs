using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class ActivitySessionSlotSubscription : IActivitySessionSlotSubscription
{
    private readonly FullDbContext _dbContext;

    public ActivitySessionSlotSubscription(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.ActivitySessionSlotSubscription>> CreateAsync(Domain.ActivitySessionSlotSubscription domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.ActivitySessionSlotSubscriptions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        var created = entity.EntityToDomain();
        await WithRanksAsync([created], ct);
        return new RepositoryResponse<Domain.ActivitySessionSlotSubscription>()
        {
            Data = created
        };
    }

    public async Task DeleteAsync(Domain.ActivitySessionSlotSubscription domain, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessionSlotSubscriptions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.ActivitySessionSlotSubscriptions.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.ActivitySessionSlotSubscription domain, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessionSlotSubscriptions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.ActivitySessionSlotSubscription>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.ActivitySessionSlotSubscriptions
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        var found = entity?.EntityToDomain();
        if (found is not null)
        {
            await WithRanksAsync([found], ct);
        }

        return new RepositoryResponse<Domain.ActivitySessionSlotSubscription>()
        {
            Data = found
        };
    }

    public async Task<RepositoryResponseRange<Domain.ActivitySessionSlotSubscription>> SearchAsync(Guid? sessionSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.ActivitySessionSlotSubscription> q = _dbContext.ActivitySessionSlotSubscriptions;
        if (sessionSlotId.HasValue)
        {
            q = q.Where(item => item.SessionSlotId.Equals(sessionSlotId.Value));
        }

        if (memberId.HasValue)
        {
            q = q.Where(item => item.MemberId.Equals(memberId.Value));
        }

        q = q.OrderBy(item => item.SessionSlotId)
            .ThenBy(item => item.IsWaiting)
            .ThenBy(item => item.Audit.CreatedAt);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        var data = entities.Select(Mapper.ActivitySessionSlotSubscription.EntityToDomain).ToList();
        await WithRanksAsync(data, ct);

        return new RepositoryResponseRange<Domain.ActivitySessionSlotSubscription>()
        {
            Data = data,
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.ActivitySessionSlotSubscriptions.CountAsync(ct)
        };
    }

    /// <summary>Inscrits confirmés et en attente, par élément (sans les noms).</summary>
    public async Task<IReadOnlyDictionary<Guid, (int Confirmed, int Waiting)>> CountByTargetAsync(IReadOnlyCollection<Guid> targetIds, CancellationToken ct)
    {
        var rows = await _dbContext.ActivitySessionSlotSubscriptions
            .Where(item => targetIds.Contains(item.SessionSlotId))
            .GroupBy(item => new { item.SessionSlotId, item.IsWaiting })
            .Select(group => new { Target = group.Key.SessionSlotId, group.Key.IsWaiting, Count = group.Count() })
            .ToListAsync(ct);

        return rows.GroupBy(row => row.Target).ToDictionary(
            group => group.Key,
            group => (group.Where(row => !row.IsWaiting).Sum(row => row.Count), group.Where(row => row.IsWaiting).Sum(row => row.Count)));
    }

    /// <summary>Le premier de la liste d'attente (le plus ancien) devient inscrit ; null si la liste est vide.</summary>
    public async Task<Domain.ActivitySessionSlotSubscription?> PromoteNextAsync(Guid targetId, CancellationToken ct)
    {
        var next = await _dbContext.ActivitySessionSlotSubscriptions
            .Where(item => item.SessionSlotId == targetId && item.IsWaiting)
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
    private async Task WithRanksAsync(List<Domain.ActivitySessionSlotSubscription> items, CancellationToken ct)
    {
        var targets = items.Where(item => item.IsWaiting).Select(item => item.SessionSlotId).Distinct().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var waiting = await _dbContext.ActivitySessionSlotSubscriptions
            .Where(item => item.IsWaiting && targets.Contains(item.SessionSlotId))
            .OrderBy(item => item.Audit.CreatedAt)
            .Select(item => new { item.Id, Target = item.SessionSlotId })
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
