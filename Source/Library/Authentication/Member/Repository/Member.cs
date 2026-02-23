using Library.Authentication;
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

    public Task DeleteAsync(Domain.Member domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Members.Remove(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public Task EditAsync(Domain.Member domain, CancellationToken ct)
    {
        var entity = domain.DomainToEntity();
        _dbContext.Members.Update(entity);
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> AskNewPasswordAsync(string email, CancellationToken ct)
    {
        bool result = true;

        Entity.Member? member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase), ct);
        if (member != null)
        {
            member.AskedNewPassword = Guid.NewGuid();
            _dbContext.Members.Update(member);
            await _dbContext.SaveChangesAsync(ct);
        }
        return result;
    }

    public async Task<bool> ChangeEmailAsync(string email, string password, CancellationToken ct)
    {
        bool result = true;

        Entity.Member? member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase), ct);
        if (member != null)
        {
            result = Encryptor.Verify(password, member.HashedPassword);
            if (result)
            {
                member.Email = email;
                _dbContext.Members.Update(member);
                await _dbContext.SaveChangesAsync(ct);
            }
        }
        return result;
    }

    public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword, CancellationToken ct)
    {
        bool result = true;

        Entity.Member? member = await _dbContext.Members.FirstOrDefaultAsync(m => m.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase), ct);
        if (member != null)
        {
            result = Encryptor.Verify(oldPassword, member.HashedPassword);
            if (result)
            {
                member.AskedNewPassword = null;
                member.HashedPassword = Encryptor.Hash(newPassword);
                _dbContext.Members.Update(member);
                await _dbContext.SaveChangesAsync(ct);
            }
        }
        return result;
    }

    public async Task<bool> CreateNewPasswordAsync(Guid askedNewPassword, CancellationToken ct)
    {
        bool result = true;

        Entity.Member? member = await _dbContext.Members.FirstOrDefaultAsync(m => m.AskedNewPassword.Equals(askedNewPassword), ct);
        if (member != null)
        {
            Guid newPassword = Guid.NewGuid();
            member.HashedPassword = Encryptor.Hash(newPassword.ToString());
            _dbContext.Members.Update(member);
            await _dbContext.SaveChangesAsync(ct);
        }
        return result;
    }

    public async Task<RepositoryResponseRange<Domain.Role>> FetchGroupsOfMemberAsync(Guid id, CancellationToken ct)
    {
        IQueryable<Entity.Role> q = _dbContext.GroupMembers
            .Where(item => item.MemberId == id)
            .Join(_dbContext.Groups,
                gm => gm.RoleId,
                r => r.Id,
                (gm, r) => new {gm, r})
            .Select(item => item.r);

        var entities = await q
            .ToListAsync(ct);

        return new RepositoryResponseRange<Domain.Role>()
        {
            Data = entities.Distinct()
             .Select(Mapper.Role.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Groups.CountAsync(ct)
        };
    }

    public async Task<RepositoryResponseRange<Domain.Permission>> FetchPermissionsOfMemberAsync(Guid id, CancellationToken ct)
    {
        IQueryable<Entity.Permission> q = _dbContext.PermissionMembers
            .Where(item => item.MemberId == id)
            .Join(_dbContext.Permissions,
                mr => mr.PermissionId,
                r => r.Id,
                (mr, r) => new {mr, r})
            .Select(item => item.r);

        var entities = await q
            .ToListAsync(ct);

        q = _dbContext.GroupMembers
            .Where(item => item.MemberId == id)
            .Join(_dbContext.GroupPermissions,
                gm => gm.RoleId,
                gr => gr.RoleId,
                (gm, gr) => new {gm, gr})
            .Select(item => item.gr)
            .Join(_dbContext.Permissions,
                gr => gr.PermissionId,
                r => r.Id,
                (gr, r) => new {gr, r})
            .Select(item => item.r);

        entities.AddRange(await q.ToListAsync(ct));

        return new RepositoryResponseRange<Domain.Permission>()
        {
            Data = entities.Distinct()
             .Select(Mapper.Permission.EntityToDomain)
             .ToList(),
            FilteredCount = entities.Count,
            TotalCount = await _dbContext.Permissions.CountAsync(ct)
        };
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
            var pattern = $"%{query}";
            q = q.Where(item =>
                EF.Functions.ILike(item.Name, pattern)
            );
        }

        q = q.OrderBy(item => EF.Functions.Collate(item.Name, "C"));

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
            FilteredCount = entities.Count,
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

    public async Task<RepositoryResponse<Domain.Member>> ValidateUserAsync(string email, string password, CancellationToken ct)
    {
        bool result = true;

        var member = await _dbContext.Members
            .FirstOrDefaultAsync(item => EF.Functions.ILike(item.Email, email), ct);

        if (member == null)
        {
            return new RepositoryResponse<Domain.Member>() { Data = null };
        }
            
        // Vérification du mot de passe dans un thread séparé (BCrypt est CPU-intensif)
        result = await Encryptor.VerifyAsync(password, member.HashedPassword, ct);

        if (!result)
        {
            return new RepositoryResponse<Domain.Member>() { Data = null };
        }

        if (member.AskedNewPassword.HasValue)
        {
            member.AskedNewPassword = null;
            _dbContext.Members.Update(member);
            await _dbContext.SaveChangesAsync(ct);
        }

        return new RepositoryResponse<Domain.Member>()
        {
            Data = member.EntityToDomain()
        };
    }
}
