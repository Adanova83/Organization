using Library.Repository;

namespace Library.Handler;

public sealed class ProjectSlot : ICrudHandler<Domain.ProjectSlot>
{
    private readonly IProjectSlot _repository;

    public ProjectSlot(IProjectSlot repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.ProjectSlot>> CreateAsync(Domain.ProjectSlot domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.ProjectSlot domain, CancellationToken ct)
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

    public Task EditAsync(Domain.ProjectSlot domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.ProjectSlot>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.ProjectSlot>> SearchAsync(Guid projectId, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return _repository.SearchAsync(projectId, page, pageSize, ct);
    }

    private void ValidateDomainRules(Domain.ProjectSlot domain)
    {
        if (domain.Capacity is < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.Capacity), "Capacity must be at least 1, or empty for no limit.");
        }
        if (domain.DayOfWeek < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.DayOfWeek), "Day of week must be greater or equal than 0.");
        }
        if (domain.DayOfWeek >= 7)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.DayOfWeek), "Day of week must be less than 7.");
        }
        if (domain.EndTime <= domain.StartTime)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.EndTime), "End time must be greater than start time.");
        }
    }
}
