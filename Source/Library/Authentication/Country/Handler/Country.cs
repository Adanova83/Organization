using Library.Repository;

namespace Library.Handler;

public sealed class Country
{
    private readonly ICountry _repository;

    public Country(ICountry repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.Country>> CreateAsync(Domain.Country domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.Country domain, CancellationToken ct)
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

    public Task EditAsync(Domain.Country domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.Country>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponse<Domain.Country>> SearchByCodeAlpaha2Async(string codeAlpha2, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(codeAlpha2))
        {
            throw new ArgumentException("Code must not be empty for search.", nameof(codeAlpha2));
        }

        return _repository.SearchByCodeAlpaha2Async(codeAlpha2, ct);
    }

    public Task<RepositoryResponse<Domain.Country>> SearchByCodeAlpaha3Async(string codeAlpha3, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(codeAlpha3))
        {
            throw new ArgumentException("Code must not be empty for search.", nameof(codeAlpha3));
        }

        return _repository.SearchByCodeAlpaha3Async(codeAlpha3, ct);
    }

    public Task<RepositoryResponseRange<Domain.Country>> SearchByNameAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return _repository.SearchByNameAsync(query, page, pageSize, ct);
    }

    private void ValidateDomainRules(Domain.Country domain)
    {
        if (string.IsNullOrWhiteSpace(domain.CodeAlpha2))
        {
            throw new ArgumentException("CodeAlpha2 is required.", nameof(domain.CodeAlpha2));
        }
        if (string.IsNullOrWhiteSpace(domain.CodeAlpha3))
        {
            throw new ArgumentException("CodeAlpha3 is required.", nameof(domain.CodeAlpha3));
        }
        if (string.IsNullOrWhiteSpace(domain.Name))
        {
            throw new ArgumentException("Name is required.", nameof(domain.Name));
        }
    }
}
