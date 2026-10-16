using Library.Repository;

namespace Library.Handler;

public sealed class ActivitySessionSlotSubscription : ICrudHandler<Domain.ActivitySessionSlotSubscription>
{
    private readonly IActivitySessionSlotSubscription _repository;

    public ActivitySessionSlotSubscription(IActivitySessionSlotSubscription repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.ActivitySessionSlotSubscription>> CreateAsync(Domain.ActivitySessionSlotSubscription domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.ActivitySessionSlotSubscription domain, CancellationToken ct)
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

    public Task EditAsync(Domain.ActivitySessionSlotSubscription domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.ActivitySessionSlotSubscription>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.ActivitySessionSlotSubscription>> SearchAsync(Guid? sessionSlotId, Guid? memberId, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return _repository.SearchAsync(sessionSlotId, memberId, page, pageSize, ct);
    }

    private void ValidateDomainRules(Domain.ActivitySessionSlotSubscription domain)
    {
        if (domain.MemberId == Guid.Empty)
        {
            throw new ArgumentException("Member ID must not be empty.", nameof(domain.MemberId));
        }
        if (domain.SessionSlotId == Guid.Empty)
        {
            throw new ArgumentException("Session slot ID must not be empty.", nameof(domain.SessionSlotId));
        }
    }
}
