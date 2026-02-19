using Library.Repository;

namespace Library.Handler;

public sealed class AccessEvent
{
    private readonly IAccessEvent _repository;

    public AccessEvent(IAccessEvent repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.AccessEvent domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.AccessEvent domain, CancellationToken ct)
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

    public Task EditAsync(Domain.AccessEvent domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.AccessEvent>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.AccessEvent>> SearchAsync(Guid badgeId, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return _repository.SearchAsync(badgeId, page, pageSize, ct);
    }

    private void ValidateDomainRules(Domain.AccessEvent domain)
    {
        if (domain.BadgeId == Guid.Empty)
        {
            throw new ArgumentException("Badge ID must not be empty.", nameof(domain.BadgeId));
        }
        if (domain.ApplicationId != Guid.Empty)
        {
            throw new ArgumentException("Application ID must not be empty.", nameof(domain.ApplicationId));
        }
        if (domain.ExitedAt.HasValue && domain.ExitedAt.Value < domain.AccessedAt)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.ExitedAt), "ExitedAt must be greater than or equal to AccessedAt.");
        }
    }
}
