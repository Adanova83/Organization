using Library.Repository;

namespace Library.Handler;

/// <summary>Agenda d'un membre : ses séances datées (ateliers, projets, événements), comme participant ou animateur.</summary>
public sealed class Agenda
{
    /// <summary>Période maximale d'une demande : une année scolaire.</summary>
    public const int MaxDays = 400;

    private readonly IAgenda _repository;

    public Agenda(IAgenda repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<Contract.AgendaEntry>> ListAsync(Guid memberId, DateOnly from, DateOnly to, bool mondayFirst, CancellationToken ct)
    {
        if (to < from)
        {
            throw new ArgumentException("The end of the period must not be before its start.", nameof(to));
        }

        if (to.DayNumber - from.DayNumber > MaxDays)
        {
            throw new ArgumentException($"The period must not exceed {MaxDays} days.", nameof(to));
        }

        return _repository.ListAsync(memberId, from, to, mondayFirst, ct);
    }
}
