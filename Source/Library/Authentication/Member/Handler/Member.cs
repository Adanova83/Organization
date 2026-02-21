using Library.Repository;

namespace Library.Handler;

public sealed class Member
{
    private readonly IMember _repository;

    public Member(IMember repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.Member>> CreateAsync(Domain.Member domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.Member domain, CancellationToken ct)
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

    public Task EditAsync(Domain.Member domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.Member>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponse<Domain.Member>> SearchByEmailAsync(string email, CancellationToken ct)
    {
        if (email == null)
        {
            throw new ArgumentException("Email must not be empty for search.", nameof(email));
        }

        return _repository.SearchByEmailAsync(email, ct);
    }

    public Task<RepositoryResponseRange<Domain.Member>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return _repository.SearchAsync(query, page, pageSize, ct);
    }

    private void ValidateDomainRules(Domain.Member domain)
    {
        if (string.IsNullOrWhiteSpace(domain.Name))
        {
            throw new ArgumentException("Name is required.", nameof(domain.Name));
        }
        if (domain.Email != null && !domain.Email.Contains('@'))
        {
            throw new ArgumentException("Email is not valid.", nameof(domain.Email));
        }
    }
}
