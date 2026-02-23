using Library.Repository;

namespace Library.Handler;

public sealed class RoleMember
{
    private readonly IRoleMember _repository;

    public RoleMember(IRoleMember repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.RoleMember domain, CancellationToken ct)
    {
        if (domain == null)
        {
            throw new ArgumentNullException(nameof(domain));
        }

        if (domain.Id != Guid.Empty)
        {
            throw new ArgumentException("ID must be empty for creation.", nameof(domain.Id));
        }

        ValidateDomainRules(domain);

        return _repository.CreateAsync(domain, ct);
    }

    public Task DeleteAsync(Domain.RoleMember domain, CancellationToken ct)
    {
        if (domain == null)
        {
            throw new ArgumentNullException(nameof(domain));
        }

        if (domain.Id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for deletion.", nameof(domain.Id));
        }

        return _repository.DeleteAsync(domain, ct);
    }

    public Task EditAsync(Domain.RoleMember domain, CancellationToken ct)
    {
        if (domain == null)
        {
            throw new ArgumentNullException(nameof(domain));
        }

        if (domain.Id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for edition.", nameof(domain.Id));
        }

        ValidateDomainRules(domain);

        return _repository.EditAsync(domain, ct);
    }

    public Task<RepositoryResponse<Domain.RoleMember>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.RoleMember>> SearchByMemberIdAsync(Guid memberId, CancellationToken ct)
    {
        if (memberId == Guid.Empty)
        {
            throw new ArgumentException("Member id is required.", nameof(memberId));
        }
        return _repository.SearchByMemberIdAsync(memberId, ct);
    }

    public Task<RepositoryResponseRange<Domain.RoleMember>> SearchByRoleIdAsync(Guid roleId, CancellationToken ct)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException("Role id is required.", nameof(roleId));
        }
        return _repository.SearchByRoleIdAsync(roleId, ct);
    }

    private void ValidateDomainRules(Domain.RoleMember domain)
    {
        if (domain.MemberId == Guid.Empty)
        {
            throw new ArgumentException("Member id is required.", nameof(domain.MemberId));
        }
        if (domain.RoleId == Guid.Empty)
        {
            throw new ArgumentException("Group id is required.", nameof(domain.RoleId));
        }
    }
}
