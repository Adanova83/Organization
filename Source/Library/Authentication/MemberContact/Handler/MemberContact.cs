using Library.Repository;

namespace Library.Handler;

public sealed class MemberContact : ICrudHandler<Domain.MemberContact>
{
    private readonly IMemberContact _repository;

    public MemberContact(IMemberContact repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.MemberContact>> CreateAsync(Domain.MemberContact domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.MemberContact domain, CancellationToken ct)
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

    public Task EditAsync(Domain.MemberContact domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.MemberContact>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.MemberContact>> SearchAsync(Guid? memberId, Guid? contactId, int page, int pageSize, CancellationToken ct)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater or equal than 0.");
        }

        if (pageSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater or equal than 0.");
        }

        return _repository.SearchAsync(memberId, contactId, page, pageSize, ct);
    }

    private static void ValidateDomainRules(Domain.MemberContact domain)
    {
        if (domain.MemberId == Guid.Empty)
        {
            throw new ArgumentException("Member ID must not be empty.", nameof(domain.MemberId));
        }

        if (domain.ContactId == Guid.Empty)
        {
            throw new ArgumentException("Contact ID must not be empty.", nameof(domain.ContactId));
        }

        if (!Enum.IsDefined(domain.Relation))
        {
            throw new ArgumentException("Unknown relation.", nameof(domain.Relation));
        }
    }
}
