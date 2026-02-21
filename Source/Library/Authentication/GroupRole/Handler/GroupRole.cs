using Library.Repository;

namespace Library.Handler;

public sealed class GroupRole
{
    private readonly IGroupRole _repository;

    public GroupRole(IGroupRole repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.GroupRole domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.GroupRole domain, CancellationToken ct)
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

    public Task EditAsync(Domain.GroupRole domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.GroupRole>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.GroupRole>> SearchByRoleIdAsync(Guid RoleId, CancellationToken ct)
    {
        if (RoleId == Guid.Empty)
        {
            throw new ArgumentException("Role id is required.", nameof(RoleId));
        }
        return _repository.SearchByRoleIdAsync(RoleId, ct);
    }

    public Task<RepositoryResponseRange<Domain.GroupRole>> SearchByGroupIdAsync(Guid GroupId, CancellationToken ct)
    {
        if (GroupId == Guid.Empty)
        {
            throw new ArgumentException("Group id is required.", nameof(GroupId));
        }
        return _repository.SearchByGroupIdAsync(GroupId, ct);
    }

    private void ValidateDomainRules(Domain.GroupRole domain)
    {
        if (domain.RoleId == Guid.Empty)
        {
            throw new ArgumentException("Role id is required.", nameof(domain.RoleId));
        }
        if (domain.GroupId == Guid.Empty)
        {
            throw new ArgumentException("Group id is required.", nameof(domain.GroupId));
        }
    }
}
