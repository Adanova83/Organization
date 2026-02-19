using Library.Repository;

namespace Library.Handler;

public sealed class ActivitySession
{
    private readonly IActivitySession _repository;

    public ActivitySession(IActivitySession repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.ActivitySession domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.ActivitySession domain, CancellationToken ct)
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

    public Task EditAsync(Domain.ActivitySession domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.ActivitySession>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.ActivitySession>> SearchAsync(Guid activityId, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return _repository.SearchAsync(activityId, page, pageSize, ct);
    }

    private void ValidateDomainRules(Domain.ActivitySession domain)
    {
        if (domain.EndDate < domain.StartDate)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.EndDate), "End date must be greater than or equal to start date.");
        }
        if (domain.Price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.Price), "Price must be greater or equal than 0.");
        }
        if (domain.ActivityId == Guid.Empty)
        {
            throw new ArgumentException("Activity ID must not be empty.", nameof(domain));
        }
        if(domain.OrganizationSessionId == Guid.Empty)
        {
            throw new ArgumentException("Organization session ID must not be empty.", nameof(domain));
        }
    }
}
