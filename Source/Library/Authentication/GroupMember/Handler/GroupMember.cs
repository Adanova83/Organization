using Library.Repository;

namespace Library.Handler;

public sealed class GroupMember
{
    private readonly IGroupMember _repository;

    public GroupMember(IGroupMember repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.GroupMember domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.GroupMember domain, CancellationToken ct)
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

    public Task EditAsync(Domain.GroupMember domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.GroupMember>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.GroupMember>> SearchByMemberIdAsync(Guid memberId, CancellationToken ct)
    {
        if (memberId == Guid.Empty)
        {
            throw new ArgumentException("Member id is required.", nameof(memberId));
        }
        return _repository.SearchByMemberIdAsync(memberId, ct);
    }

    public Task<RepositoryResponseRange<Domain.GroupMember>> SearchByGroupIdAsync(Guid GroupId, CancellationToken ct)
    {
        if (GroupId == Guid.Empty)
        {
            throw new ArgumentException("Group id is required.", nameof(GroupId));
        }
        return _repository.SearchByGroupIdAsync(GroupId, ct);
    }

    private void ValidateDomainRules(Domain.GroupMember domain)
    {
        if (domain.MemberId == Guid.Empty)
        {
            throw new ArgumentException("Member id is required.", nameof(domain.MemberId));
        }
        if (domain.GroupId == Guid.Empty)
        {
            throw new ArgumentException("Group id is required.", nameof(domain.GroupId));
        }
    }
}
