namespace Library.Repository;

/// <summary>Agenda d'un membre : ses séances datées comme participant ou comme animateur.</summary>
public interface IAgenda
{
    /// <summary>Séances du membre entre deux dates (incluses), par date et heure.</summary>
    Task<IReadOnlyList<Contract.AgendaEntry>> ListAsync(Guid memberId, DateOnly from, DateOnly to, bool mondayFirst, CancellationToken ct);
}
