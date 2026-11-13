using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Contact : IContact
{
    private readonly FullDbContext _dbContext;

    public Contact(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Contact>> CreateAsync(Domain.Contact domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Contacts.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Contact>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Contact domain, CancellationToken ct)
    {
        var entity = await _dbContext.Contacts.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Contacts.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Contact domain, CancellationToken ct)
    {
        var entity = await _dbContext.Contacts.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.UpdateFrom(entity, domain.DomainToEntity());
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Contact>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Contacts
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Contact>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    /// <summary>Recherche par nom, prénom, téléphone ou e-mail.</summary>
    public async Task<RepositoryResponseRange<Domain.Contact>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Contact> q = _dbContext.Contacts;
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}%";
            q = q.Where(item =>
                EF.Functions.ILike(item.Lastname, pattern)
                || (item.Firstname != null && EF.Functions.ILike(item.Firstname, pattern))
                || (item.PhoneNumber != null && EF.Functions.ILike(item.PhoneNumber, pattern))
                || (item.Email != null && EF.Functions.ILike(item.Email, pattern)));
        }

        q = q.OrderBy(item => item.Lastname).ThenBy(item => item.Firstname);

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q.ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Contact>()
        {
            Data = entities.Select(Mapper.Contact.EntityToDomain).ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Contacts.CountAsync(ct)
        };
    }
}
