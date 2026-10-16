using Library.Repository;

namespace Library.Handler;

public sealed class OrganizationSession : ICrudHandler<Domain.OrganizationSession>
{
    private readonly IOrganizationSession _repository;

    public OrganizationSession(IOrganizationSession repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.OrganizationSession>> CreateAsync(Domain.OrganizationSession domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.OrganizationSession domain, CancellationToken ct)
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

    public Task EditAsync(Domain.OrganizationSession domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.OrganizationSession>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.OrganizationSession>> SearchAsync(Guid OrganizationId, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return _repository.SearchAsync(OrganizationId, page, pageSize, ct);
    }

    public Task<RepositoryResponseRange<Domain.OrganizationSession>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct)
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

    private void ValidateDomainRules(Domain.OrganizationSession domain)
    {
        if (domain.SchoolYearId == Guid.Empty)
        {
            throw new ArgumentException("School year ID must not be empty.", nameof(domain.SchoolYearId));
        }
        if (domain.Price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.Price), "Price must be greater or equal than 0.");
        }
        if (domain.OrganizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization ID must not be empty.", nameof(domain));
        }
    }
}
