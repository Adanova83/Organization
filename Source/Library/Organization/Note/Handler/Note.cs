using Library.Repository;

namespace Library.Handler;

public sealed class Note
{
    private readonly INote _repository;

    public Note(INote repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.Note domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.Note domain, CancellationToken ct)
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

    public Task EditAsync(Domain.Note domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.Note>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    private void ValidateDomainRules(Domain.Note domain)
    {
        if (string.IsNullOrWhiteSpace(domain.Title))
        {
            throw new ArgumentException("Title is required.", nameof(domain.Title));
        }
        if (string.IsNullOrWhiteSpace(domain.Body))
        {
            throw new ArgumentException("Body is required.", nameof(domain.Body));
        }
        if (string.IsNullOrWhiteSpace(domain.RGBBackgroundColor))
        {
            throw new ArgumentException("RGBBackgroundColor is required.", nameof(domain.RGBBackgroundColor));
        }
        if (domain.MemberId == Guid.Empty)
        {
            throw new ArgumentException("Member ID must not be empty.", nameof(domain.MemberId));
        }
    }
}
