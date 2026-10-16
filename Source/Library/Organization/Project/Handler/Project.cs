using Library.Repository;

namespace Library.Handler;

public sealed class Project : ICrudHandler<Domain.Project>
{
    private readonly IProject _repository;

    public Project(IProject repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.Project>> CreateAsync(Domain.Project domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.Project domain, CancellationToken ct)
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

    public Task EditAsync(Domain.Project domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.Project>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.Project>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
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

    public Task<RepositoryResponseRange<Domain.Project>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct)
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

    private void ValidateDomainRules(Domain.Project domain)
    {
        if (string.IsNullOrWhiteSpace(domain.Code))
        {
            throw new ArgumentException("Code is required.", nameof(domain.Code));
        }
        if (string.IsNullOrWhiteSpace(domain.Name))
        {
            throw new ArgumentException("Name is required.", nameof(domain.Name));
        }
        if (domain.AgeMinimum < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.AgeMinimum), "Age minimum must be greater or equal than 0.");
        }
        if (domain.OrganizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization ID must not be empty.", nameof(domain.OrganizationId));
        }
        if (domain.SchoolYearId == Guid.Empty)
        {
            throw new ArgumentException("School year ID must not be empty.", nameof(domain.SchoolYearId));
        }
        // Dates facultatives (sinon celles de l'année scolaire) ; si les deux sont données, la fin suit le début.
        if (domain.EndDate < domain.StartDate)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.EndDate), "End date must be greater than or equal to start date.");
        }
    }
}
