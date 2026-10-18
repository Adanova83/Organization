namespace Library.Repository;

/// <summary>Présences aux séances : créneaux d'ateliers (à une date) et événements.</summary>
public interface IAttendance
{
    /// <summary>Dates des séances du créneau ou de l'événement (null si la cible n'existe pas).</summary>
    Task<IReadOnlyList<DateOnly>?> OccurrenceDatesAsync(Contract.SubscriptionTargetKind kind, Guid targetId, bool mondayFirst, CancellationToken ct);

    /// <summary>Séances et décompte de leurs présences (null si la cible n'existe pas).</summary>
    Task<Contract.AttendanceOccurrences?> OccurrencesAsync(Contract.SubscriptionTargetKind kind, Guid targetId, bool mondayFirst, CancellationToken ct);

    /// <summary>Feuille d'émargement d'une séance (null si la cible n'existe pas).</summary>
    Task<Contract.AttendanceSheet?> SheetAsync(Contract.SubscriptionTargetKind kind, Guid targetId, DateOnly date, CancellationToken ct);

    /// <summary>Enregistre (ou remplace) la présence de chaque membre indiqué.</summary>
    Task RecordAsync(
        Contract.SubscriptionTargetKind kind, Guid targetId, DateOnly date,
        IReadOnlyCollection<Contract.AttendanceEntry> entries, Contract.AttendanceMethod method, CancellationToken ct);

    /// <summary>Fréquentation par créneau / événement et par membre, sur une période.</summary>
    Task<Contract.AttendanceStats> StatsAsync(DateOnly from, DateOnly to, CancellationToken ct);
}
