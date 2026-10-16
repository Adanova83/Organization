using Library.Repository;

namespace Library.Handler;

public sealed class Event : ICrudHandler<Domain.Event>
{
    private readonly IEvent _repository;

    public Event(IEvent repository)
    {
        _repository = repository;
    }

    public Task<RepositoryResponse<Domain.Event>> CreateAsync(Domain.Event domain, CancellationToken ct)
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

    public Task DeleteAsync(Domain.Event domain, CancellationToken ct)
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

    public Task EditAsync(Domain.Event domain, CancellationToken ct)
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

    public Task<RepositoryResponse<Domain.Event>> SearchAsync(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID must not be empty for search.", nameof(id));
        }

        return _repository.SearchAsync(id, ct);
    }

    public Task<RepositoryResponseRange<Domain.Event>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct)
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

    public Task<RepositoryResponseRange<Domain.Event>> SearchBySchoolYearAsync(Guid schoolYearId, int page, int pageSize, CancellationToken ct)
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

    public Task<RepositoryResponseRange<Domain.Event>> SearchBySeriesAsync(Guid seriesId, CancellationToken ct)
    {
        if (seriesId == Guid.Empty)
        {
            throw new ArgumentException("Series ID must not be empty for search.", nameof(seriesId));
        }

        return _repository.SearchBySeriesAsync(seriesId, ct);
    }

    /// <summary>Crée les occurrences d'une série : elles reçoivent un même identifiant de série et un code suffixé par leur date.</summary>
    public Task<IReadOnlyList<Domain.Event>> CreateSeriesAsync(IReadOnlyList<Domain.Event> occurrences, DateOnly[] localDates, CancellationToken ct)
    {
        if (occurrences.Count == 0 || occurrences.Count != localDates.Length)
        {
            throw new ArgumentException("A series needs at least one occurrence, each with its date.", nameof(occurrences));
        }

        var seriesId = Guid.NewGuid();
        for (var index = 0; index < occurrences.Count; index++)
        {
            var occurrence = occurrences[index];
            if (occurrence.Id != Guid.Empty)
            {
                throw new ArgumentException("ID must be empty for creation.", nameof(occurrence.Id));
            }

            ValidateDomainRules(occurrence);

            // Codes uniques : code de la série + date (50 caractères au plus).
            var baseCode = occurrence.Code.Trim();
            occurrence.Code = $"{baseCode[..Math.Min(baseCode.Length, 41)]}-{localDates[index]:yyyyMMdd}";
            occurrence.SeriesId = seriesId;
        }

        return _repository.CreateRangeAsync(occurrences, ct);
    }

    private void ValidateDomainRules(Domain.Event domain)
    {
        if (domain.Capacity is < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.Capacity), "Capacity must be at least 1, or empty for no limit.");
        }
        if (string.IsNullOrWhiteSpace(domain.Code))
        {
            throw new ArgumentException("Code is required.", nameof(domain.Code));
        }
        if (string.IsNullOrWhiteSpace(domain.Name))
        {
            throw new ArgumentException("Name is required.", nameof(domain.Name));
        }
        if (domain.EndDate < domain.StartDate)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.EndDate), "End date must be greater than or equal to start date.");
        }
        if (domain.AgeMinimum.HasValue && domain.AgeMinimum.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(domain.AgeMinimum), "Age minimum must be greater or equal than 0.");
        }
        if (domain.OrganizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization ID is required.", nameof(domain.OrganizationId));
        }
        if (domain.SchoolYearId == Guid.Empty)
        {
            throw new ArgumentException("School year ID is required.", nameof(domain.SchoolYearId));
        }
    }
}
