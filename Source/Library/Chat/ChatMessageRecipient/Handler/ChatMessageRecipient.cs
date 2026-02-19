using Library.Repository;

namespace Library.Handler;

public sealed class ChatMessageRecipient
{
    private readonly IChatMessageRecipient _repository;

    public ChatMessageRecipient(IChatMessageRecipient repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.ChatMessageRecipient domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.ChatMessageRecipient domain, CancellationToken ct)
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

    public Task EditAsync(Domain.ChatMessageRecipient domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.ChatMessageRecipient>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    private void ValidateDomainRules(Domain.ChatMessageRecipient domain)
    {
        if (domain.ChatMessageId == Guid.Empty)
        {
            throw new ArgumentException("Chat Message ID must not be empty.", nameof(domain.ChatMessageId));
        }
        if (domain.RecipentId == Guid.Empty)
        {
            throw new ArgumentException("Recipent ID must not be empty.", nameof(domain.RecipentId));
        }
    }
}
