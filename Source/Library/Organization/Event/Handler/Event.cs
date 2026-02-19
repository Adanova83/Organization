using Library.Repository;

namespace Library.Handler;

public sealed class Event
{
    private readonly IEvent _repository;

    public Event(IEvent repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.Event domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.Event domain, CancellationToken ct)
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

    public Task EditAsync(Domain.Event domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.Event>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.Event>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
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

    private void ValidateDomainRules(Domain.Event domain)
    {
        if (string.IsNullOrWhiteSpace(domain.Code))
        {
            throw new ArgumentException("Code is required.", nameof(domain.Code));
        }
        if (string.IsNullOrWhiteSpace(domain.Name))
        {
            throw new ArgumentException("Name is required.", nameof(domain.Name));
        }
        if (domain.EndDate < domain.StartDate)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.EndDate), "End date must be greater than or equal to start date.");
        }
        if (domain.AgeMinimum.HasValue && domain.AgeMinimum.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.AgeMinimum), "Age minimum must be greater or equal than 0.");
        }
        if (domain.OrganizationSessionId == Guid.Empty)
        {
            throw new ArgumentException("Organization ID is required.", nameof(domain.OrganizationSessionId));
        }
    }
}
