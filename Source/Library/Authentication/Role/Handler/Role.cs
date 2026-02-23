using Library.Repository;

namespace Library.Handler;

public sealed class Role
{
    private readonly IRole _repository;

    public Role(IRole repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.Role>> CreateAsync(Domain.Role domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.Role domain, CancellationToken ct)
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

    public Task EditAsync(Domain.Role domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.Role>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponse<Domain.Role>> SearchByCodeAsync(string code, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code must not be empty for search.", nameof(code));
        }

        return _repository.SearchByCodeAsync(code, ct);
    }

    public Task<RepositoryResponse<Domain.Role>> SearchByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Code must not be empty for search.", nameof(name));
        }

        return _repository.SearchByNameAsync(name, ct);
    }

    private void ValidateDomainRules(Domain.Role domain)
    {
        if (string.IsNullOrWhiteSpace(domain.Code))
        {
            throw new ArgumentException("Code is required.", nameof(domain.Code));
        }
        if (string.IsNullOrWhiteSpace(domain.Name))
        {
            throw new ArgumentException("Name is required.", nameof(domain.Name));
        }
    }
}
