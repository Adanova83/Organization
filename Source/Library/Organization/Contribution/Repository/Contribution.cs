using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Contribution : IContribution
{
    private readonly FullDbContext _dbContext;

    public Contribution(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Contribution>> CreateAsync(Domain.Contribution domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Contributions.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Contribution>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Contribution domain, CancellationToken ct)
    {
        var entity = await _dbContext.Contributions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Contributions.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Contribution domain, CancellationToken ct)
    {
        var entity = await _dbContext.Contributions.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Contribution>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Contributions
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Contribution>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Contribution>> SearchAsync(Guid memberId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Contribution> q = _dbContext.Contributions;

        q = q.Where(item => item.MemberId == memberId)
            .OrderBy(item => item.PayedDate);

        var filteredCount = await q.CountAsync(ct);

        if(page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Contribution>()
        {
            Data = entities
             .Select(Mapper.Contribution.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Contributions.CountAsync(ct)
        };
    }

    public async Task<Contract.SeasonContributions?> SeasonAsync(Guid organizationSessionId, CancellationToken ct)
    {
        var season = await _dbContext.OrganizationSessions
            .Where(item => item.Id == organizationSessionId)
            .Select(item => new { item.Id, item.OrganizationId, item.SchoolYearId, item.Price, item.SchoolYear.Code })
            .FirstOrDefaultAsync(ct);
        if (season == null)
        {
            return null;
        }

        // Paiements de la saison, par membre.
        var payments = (await _dbContext.Contributions
                .Where(item => item.OrganizationSessionId == season.Id)
                .Select(item => new { item.MemberId, item.Amount, item.PayedDate })
                .ToListAsync(ct))
            .GroupBy(item => item.MemberId)
            .ToDictionary(group => group.Key, group => (Paid: group.Sum(item => item.Amount), Last: group.Max(item => item.PayedDate)));

        // Participants : inscrits (ou en attente) à un atelier, un événement ou un projet de l'organisation cette année-là.
        var participants = new HashSet<Guid>(await _dbContext.ActivitySessionSlotSubscriptions
            .Where(item => item.SessionSlot.Session.SchoolYearId == season.SchoolYearId
                && item.SessionSlot.Session.Activity.OrganizationId == season.OrganizationId)
            .Select(item => item.MemberId)
            .ToListAsync(ct));
        participants.UnionWith(await _dbContext.EventSubscriptions
            .Where(item => item.Event.SchoolYearId == season.SchoolYearId && item.Event.OrganizationId == season.OrganizationId)
            .Select(item => item.MemberId)
            .ToListAsync(ct));
        participants.UnionWith(await _dbContext.ProjectSlotSubscriptions
            .Where(item => item.ProjectSlot.Project.SchoolYearId == season.SchoolYearId
                && item.ProjectSlot.Project.OrganizationId == season.OrganizationId)
            .Select(item => item.MemberId)
            .ToListAsync(ct));

        // Membres actuels, plus ceux qui ont payé ou participent même s'ils ne sont plus actifs.
        var now = DateTime.UtcNow;
        var members = (await _dbContext.Members
                .Select(item => new { item.Id, item.Lastname, item.Firstname, item.Active.Until })
                .ToListAsync(ct))
            .Where(item => item.Until == null || item.Until >= now || payments.ContainsKey(item.Id) || participants.Contains(item.Id))
            .OrderBy(item => item.Lastname).ThenBy(item => item.Firstname);

        return new Contract.SeasonContributions
        {
            OrganizationSessionId = season.Id,
            OrganizationId = season.OrganizationId,
            SchoolYearCode = season.Code,
            Price = season.Price,
            Members = members.Select(item => new Contract.MemberContribution
            {
                MemberId = item.Id,
                Lastname = item.Lastname,
                Firstname = item.Firstname,
                Participates = participants.Contains(item.Id),
                Due = season.Price,
                Paid = payments.TryGetValue(item.Id, out var paid) ? paid.Paid : 0,
                LastPayedDate = payments.TryGetValue(item.Id, out var last) ? last.Last : null
            }).ToList()
        };
    }
}
