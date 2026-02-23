using Library.Repository;

namespace Library.Handler;

public sealed class Permission
{
    private readonly IPermission _repository;

    public Permission(IPermission repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.Permission>> CreateAsync(Domain.Permission domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.Permission domain, CancellationToken ct)
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

    public Task EditAsync(Domain.Permission domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.Permission>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponse<Domain.Permission>> SearchByCodeAsync(string type, string value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Type must not be empty for search.", nameof(type));
        }
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value must not be empty for search.", nameof(value));
        }

        return _repository.SearchByCodeAsync(type, value, ct);
    }

    public Task<RepositoryResponse<Domain.Permission>> SearchByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name must not be empty for search.", nameof(name));
        }

        return _repository.SearchByNameAsync(name, ct);
    }

    private void ValidateDomainRules(Domain.Permission domain)
    {
        if (string.IsNullOrWhiteSpace(domain.Type))
        {
            throw new ArgumentException("Code is required.", nameof(domain.Type));
        }
        if (string.IsNullOrWhiteSpace(domain.Value))
        {
            throw new ArgumentException("Name is required.", nameof(domain.Value));
        }
    }
}
