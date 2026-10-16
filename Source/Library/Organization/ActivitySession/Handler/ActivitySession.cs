using Library.Repository;

namespace Library.Handler;

public sealed class ActivitySession : ICrudHandler<Domain.ActivitySession>
{
    private readonly IActivitySession _repository;

    public ActivitySession(IActivitySession repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.ActivitySession>> CreateAsync(Domain.ActivitySession domain, CancellationToken ct)
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

    public Task<RepositoryResponseRange<Domain.ActivitySession>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct)
    {
        if (schoolYearId == Guid.Empty)
        {
            throw new ArgumentException("School year ID must not be empty for search.", nameof(schoolYearId));
        }

        if (page < 0 || pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page and page size must be greater or equal than 0.");
        }

        return _repository.SearchBySchoolYearAsync(schoolYearId, page, pageSize, ct);
    }

    private void ValidateDomainRules(Domain.ActivitySession domain)
    {
        // Dates facultatives (sinon celles de l'année scolaire) ; si les deux sont données, la fin suit le début.
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
        if (domain.SchoolYearId == Guid.Empty)
        {
            throw new ArgumentException("School year ID must not be empty.", nameof(domain.SchoolYearId));
        }
        if (!domain.DuringTermTime && !domain.DuringSchoolHolidays)
        {
            throw new ArgumentException("The session must take place during term time, during school holidays, or both.", nameof(domain.DuringTermTime));
        }
    }
}
