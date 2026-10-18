namespace Library.Repository;

/// <summary>Présences aux séances des ateliers (créneau + date).</summary>
public interface IAttendance
{
    /// <summary>Dates des séances du créneau (null si le créneau n'existe pas).</summary>
    Task<IReadOnlyList<DateOnly>?> OccurrenceDatesAsync(Guid slotId, bool mondayFirst, CancellationToken ct);

    /// <summary>Séances et décompte de leurs présences (null si le créneau n'existe pas).</summary>
    Task<Contract.AttendanceOccurrences?> OccurrencesAsync(Guid slotId, bool mondayFirst, CancellationToken ct);

    /// <summary>Feuille d'émargement d'une séance (null si le créneau n'existe pas).</summary>
    Task<Contract.AttendanceSheet?> SheetAsync(Guid slotId, DateOnly date, CancellationToken ct);

    /// <summary>Enregistre (ou remplace) la présence de chaque membre indiqué.</summary>
    Task RecordAsync(Guid slotId, DateOnly date, IReadOnlyCollection<Contract.AttendanceEntry> entries, Contract.AttendanceMethod method, CancellationToken ct);

    /// <summary>Fréquentation par créneau et par membre, sur une période.</summary>
    Task<Contract.AttendanceStats> StatsAsync(DateOnly from, DateOnly to, CancellationToken ct);
}
