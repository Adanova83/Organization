using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class MemberContact : IMemberContact
{
    private readonly FullDbContext _dbContext;

    public MemberContact(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.MemberContact>> CreateAsync(Domain.MemberContact domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.MemberContacts.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return await SearchAsync(entity.Id, ct);
    }

    public async Task DeleteAsync(Domain.MemberContact domain, CancellationToken ct)
    {
        var entity = await _dbContext.MemberContacts.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.MemberContacts.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.MemberContact domain, CancellationToken ct)
    {
        var entity = await _dbContext.MemberContacts.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.MemberContact>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.MemberContacts
            .Include(item => item.Contact)
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.MemberContact>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.MemberContact>> SearchAsync(Guid? memberId, Guid? contactId, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.MemberContact> q = _dbContext.MemberContacts.Include(item => item.Contact);
        if (memberId.HasValue)
        {
            q = q.Where(item => item.MemberId == memberId.Value);
        }

        if (contactId.HasValue)
        {
            q = q.Where(item => item.ContactId == contactId.Value);
        }

        // Contacts à prévenir en cas d'urgence d'abord.
        q = q.OrderByDescending(item => item.IsEmergencyContact)
            .ThenBy(item => item.Contact.Lastname)
            .ThenBy(item => item.Contact.Firstname);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q.ToListAsync(ct);

        return new RepositoryResponseRange<Domain.MemberContact>()
        {
            Data = entities.Select(Mapper.MemberContact.EntityToDomain).ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.MemberContacts.CountAsync(ct)
        };
    }

    public async Task<IReadOnlyList<Domain.MemberContact>> ByMembersAsync(IReadOnlyCollection<Guid> memberIds, CancellationToken ct)
    {
        var entities = await _dbContext.MemberContacts
            .Include(item => item.Contact)
            .Where(item => memberIds.Contains(item.MemberId))
            .OrderByDescending(item => item.IsEmergencyContact)
            .ThenBy(item => item.Contact.Lastname)
            .ToListAsync(ct);

        return entities.Select(Mapper.MemberContact.EntityToDomain).ToList();
    }
}
