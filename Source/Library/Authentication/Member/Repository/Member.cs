using Library.Data;
using Library.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Member : IMember
{
    private readonly FullDbContext _dbContext;

    public Member(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RepositoryResponse<Domain.Member>> CreateAsync(Domain.Member domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        entity.Id = Guid.NewGuid();
        _dbContext.Members.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        return new RepositoryResponse<Domain.Member>()
        {
            Data = entity.EntityToDomain()
        };
    }

    public async Task DeleteAsync(Domain.Member domain, CancellationToken ct)
    {
        var entity = await _dbContext.Members.FindAsync(domain.Id);
        if (entity != null)
        {
            _dbContext.Members.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task EditAsync(Domain.Member domain, CancellationToken ct)
    {
        var entity = await _dbContext.Members.FindAsync(domain.Id);
        if (entity != null)
        {
            var updated = domain.DomainToEntity();
            _dbContext.UpdateFrom(entity, updated);

            // Addr est un type détenu : SetValues ne le copie pas.
            entity.Addr.Line1 = updated.Addr.Line1;
            entity.Addr.Line2 = updated.Addr.Line2;
            entity.Addr.Line3 = updated.Addr.Line3;
            entity.Addr.City = updated.Addr.City;
            entity.Addr.PostalCode = updated.Addr.PostalCode;
            entity.Addr.CountryId = updated.Addr.CountryId;

            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<RepositoryResponse<Domain.Member>> SearchAsync(Guid id, CancellationToken ct)
    {
        var entity = await _dbContext.Members
            .FirstOrDefaultAsync(item => item.Id == id, ct);
        return new RepositoryResponse<Domain.Member>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponseRange<Domain.Member>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Entity.Member> q = _dbContext.Members;
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}%";
            q = q.Where(item =>
                EF.Functions.ILike(item.Lastname + " " + (item.Firstname ?? ""), pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Lastname, "C"))
            .ThenBy(item => EF.Functions.Collate(item.Firstname, "C"));

        var filteredCount = await q.CountAsync(ct);

        if (page > 0 && pageSize > 0)
        {
            q = q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Member>()
        {
            Data = entities
             .Select(Mapper.Member.EntityToDomain)
             .ToList(),
            FilteredCount = filteredCount,
            TotalCount = await _dbContext.Members.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponse<Domain.Member>> SearchByEmailAsync(string email, CancellationToken ct)
    {
        var entity = await _dbContext.Members
            .FirstOrDefaultAsync(item => item.Email.ToUpper() == email.ToUpper(), ct);
        return new RepositoryResponse<Domain.Member>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Member>> SearchByExternalIdAsync(string externalId, CancellationToken ct)
    {
        var entity = await _dbContext.Members
            .FirstOrDefaultAsync(item => item.ExternalId == externalId, ct);
        return new RepositoryResponse<Domain.Member>()
        {
            Data = entity?.EntityToDomain()
        };
    }

    public async Task<RepositoryResponse<Domain.Member>> LinkExternalAccountAsync(string externalId, string email, CancellationToken ct)
    {
        var entity = await _dbContext.Members
            .FirstOrDefaultAsync(item => item.ExternalId == externalId, ct);

        if (entity == null)
        {
            // Premier passage : rattachement par e-mail, uniquement si le membre n'est lié à aucun compte externe
            // (on ne reprend jamais un membre déjà lié à un autre compte).
            string normalizedEmail = email.ToUpper();
            entity = await _dbContext.Members
                .FirstOrDefaultAsync(item => item.Email.ToUpper() == normalizedEmail && item.ExternalId == null, ct);

            if (entity != null)
            {
                entity.ExternalId = externalId;
                await _dbContext.SaveChangesAsync(ct);
            }
        }

        return new RepositoryResponse<Domain.Member>()
        {
            Data = entity?.EntityToDomain()
        };
    }
}
