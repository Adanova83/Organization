using Library.Repository;

namespace Library.Handler;

public sealed class RolePermission
{
    private readonly IRolePermission _repository;

    public RolePermission(IRolePermission repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.RolePermission domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.RolePermission domain, CancellationToken ct)
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

    public Task EditAsync(Domain.RolePermission domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.RolePermission>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.RolePermission>> SearchByPermissionIdAsync(Guid permissionId, CancellationToken ct)
    {
        if (permissionId == Guid.Empty)
        {
            throw new ArgumentException("Permission id is required.", nameof(permissionId));
        }
        return _repository.SearchByPermissionIdAsync(permissionId, ct);
    }

    public Task<RepositoryResponseRange<Domain.RolePermission>> SearchByRoleIdAsync(Guid roleId, CancellationToken ct)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException("Role id is required.", nameof(roleId));
        }
        return _repository.SearchByRoleIdAsync(roleId, ct);
    }

    private void ValidateDomainRules(Domain.RolePermission domain)
    {
        if (domain.PermissionId == Guid.Empty)
        {
            throw new ArgumentException("Permission id is required.", nameof(domain.PermissionId));
        }
        if (domain.RoleId == Guid.Empty)
        {
            throw new ArgumentException("Role id is required.", nameof(domain.RoleId));
        }
    }
}
