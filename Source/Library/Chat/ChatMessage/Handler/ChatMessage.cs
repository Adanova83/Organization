using Library.Repository;

namespace Library.Handler;

public sealed class ChatMessage
{
    private readonly IChatMessage _repository;

    public ChatMessage(IChatMessage repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.ChatMessage domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.ChatMessage domain, CancellationToken ct)
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

    public Task EditAsync(Domain.ChatMessage domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.ChatMessage>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.ChatMessage>> SearchAsync(Guid memberId, DateOnly dateFrom, DateOnly dateTo, CancellationToken ct)
    {
        if (dateTo < dateFrom)
        {
            throw new ArgumentOutOfRangeException(nameof(dateTo), "End date must be greater than or equal to start date.");
        }

        return _repository.SearchAsync(memberId, dateFrom, dateTo, ct);
    }

    private void ValidateDomainRules(Domain.ChatMessage domain)
    {
        if (domain.SenderId == Guid.Empty)
        {
            throw new ArgumentException("Sender ID must not be empty.", nameof(domain.SenderId));
        }
        if (domain.Title == string.Empty)
        {
            throw new ArgumentException("Title must not be empty.", nameof(domain.Title));
        }
    }
}
