using Library.Repository;

namespace Library.Handler;

public sealed class BadgeAssignation
{
    private readonly IBadgeAssignation _repository;

    public BadgeAssignation(IBadgeAssignation repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.BadgeAssignation domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.BadgeAssignation domain, CancellationToken ct)
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

    public Task EditAsync(Domain.BadgeAssignation domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.BadgeAssignation>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.BadgeAssignation>> SearchAsync(Guid? badgeId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return _repository.SearchAsync(badgeId, memberId, page, pageSize, ct);
    }

    private void ValidateDomainRules(Domain.BadgeAssignation domain)
    {
        if (domain.BadgeId == Guid.Empty)
        {
            throw new ArgumentException("Badge ID must not be empty.", nameof(domain.BadgeId));
        }
        if (domain.MemberId == Guid.Empty)
        {
            throw new ArgumentException("Member ID must not be empty.", nameof(domain.MemberId));
        }
    }
}
