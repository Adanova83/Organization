using Library.Repository;

namespace Library.Handler;

public sealed class NoteLink
{
    private readonly INoteLink _repository;

    public NoteLink(INoteLink repository)
    {
        _repository = repository;
    }

    public Task CreateAsync(Domain.NoteLink domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.NoteLink domain, CancellationToken ct)
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

    public Task EditAsync(Domain.NoteLink domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.NoteLink>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    private void ValidateDomainRules(Domain.NoteLink domain)
    {
        if (domain.LinkedId == Guid.Empty)
        {
            throw new ArgumentException("Linked ID must not be empty.", nameof(domain.LinkedId));
        }
        if (string.IsNullOrWhiteSpace(domain.LinkedType))
        {
            throw new ArgumentException("Linked Type is required.", nameof(domain.LinkedType));
        }
        if (domain.NoteId == Guid.Empty)
        {
            throw new ArgumentException("Note ID must not be empty.", nameof(domain.NoteId));
        }
    }
}
