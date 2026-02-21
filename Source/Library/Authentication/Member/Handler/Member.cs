using Library.Repository;

namespace Library.Handler;

public sealed class Member
{
    private readonly IMember _repository;

    public Member(IMember repository)
    {
        _repository = repository;
    }

    public async Task<RepositoryResponse<Domain.Member>> CreateAsync(Domain.Member domain, CancellationToken ct)
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

        return await _repository.CreateAsync(domain, ct);
    }

    public async Task DeleteAsync(Domain.Member domain, CancellationToken ct)
    {
        if (domain == null)
        {
            throw new ArgumentNullException(nameof(domain));
        }

        if (domain.Id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for deletion.", nameof(domain.Id));
        }

        await _repository.DeleteAsync(domain, ct);
    }

    public async Task EditAsync(Domain.Member domain, CancellationToken ct)
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

        await _repository.EditAsync(domain, ct);
    }

    public async Task<RepositoryResponseRange<Domain.Role>> FetchRolesOfMemberAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return await _repository.FetchRolesOfMemberAsync(id, ct);
    }

    public async Task<RepositoryResponse<Domain.Member>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return await _repository.SearchAsync(id, ct);
    }

    public async Task<RepositoryResponse<Domain.Member>> SearchByEmailAsync(string email, CancellationToken ct)
    {
        if (email == null)
        {
            throw new ArgumentException("Email must not be empty for search.", nameof(email));
        }

        return await _repository.SearchByEmailAsync(email, ct);
    }

    public async Task<RepositoryResponseRange<Domain.Member>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return await _repository.SearchAsync(query, page, pageSize, ct);
    }

    public async Task<RepositoryResponse<Domain.Member>> ValidateUserAsync(string email, string password, CancellationToken ct)
    {
        if (email == null)
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }
        if (password == null)
        {
            throw new ArgumentException("Password is required.", nameof(password));
        }
        return await _repository.ValidateUserAsync(email, password, ct);
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
