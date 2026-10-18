using Library.Repository;
using Ct = Library.Contract;

namespace Library.Handler;

/// <summary>
/// Présences aux séances des ateliers (créneau + date) et des événements. Vérifie que la séance existe à cette date
/// (rythme du créneau, vacances, jours fériés) et que l'appel ne concerne que des participants de la séance.
/// </summary>
public sealed class Attendance
{
    private readonly IAttendance _repository;

    public Attendance(IAttendance repository)
    {
        _repository = repository;
    }

    public Task<Ct.AttendanceOccurrences?> OccurrencesAsync(Ct.SubscriptionTargetKind kind, Guid targetId, bool mondayFirst, CancellationToken ct)
    {
        EnsureKind(kind);
        return _repository.OccurrencesAsync(kind, targetId, mondayFirst, ct);
    }

    /// <summary>Feuille d'émargement ; null si la cible n'existe pas. Refuse une date où il n'y a pas de séance.</summary>
    public async Task<Ct.AttendanceSheet?> SheetAsync(Ct.SubscriptionTargetKind kind, Guid targetId, DateOnly date, bool mondayFirst, CancellationToken ct)
    {
        EnsureKind(kind);
        if (!await EnsureDateAsync(kind, targetId, date, mondayFirst, ct))
        {
            return null;
        }

        return await _repository.SheetAsync(kind, targetId, date, ct);
    }

    /// <summary>Enregistre l'appel ; false si la cible n'existe pas.</summary>
    public async Task<bool> RecordAsync(
        Ct.SubscriptionTargetKind kind, Guid targetId, DateOnly date,
        IReadOnlyCollection<Ct.AttendanceEntry> entries, Ct.AttendanceMethod method, bool mondayFirst, CancellationToken ct)
    {
        EnsureKind(kind);
        if (entries.Count == 0)
        {
            throw new ArgumentException("At least one attendance is required.", nameof(entries));
        }

        if (entries.Select(e => e.MemberId).Distinct().Count() != entries.Count)
        {
            throw new ArgumentException("A member appears twice.", nameof(entries));
        }

        if (entries.Any(e => !Enum.IsDefined(e.Status)))
        {
            throw new ArgumentException("Unknown attendance status.", nameof(entries));
        }

        if (!await EnsureDateAsync(kind, targetId, date, mondayFirst, ct))
        {
            return false;
        }

        // Seuls les participants de la séance (inscrits ce jour-là, ou déjà pointés) figurent sur la feuille.
        var sheet = await _repository.SheetAsync(kind, targetId, date, ct);
        var allowed = sheet!.Rows.Select(row => row.MemberId).ToHashSet();
        if (entries.Any(e => !allowed.Contains(e.MemberId)))
        {
            throw new ArgumentException("Only the participants of this session can be marked.", nameof(entries));
        }

        await _repository.RecordAsync(kind, targetId, date, entries, method, ct);
        return true;
    }

    public Task<Ct.AttendanceStats> StatsAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        if (to < from)
        {
            throw new ArgumentException("The end of the period must not be before its start.", nameof(to));
        }

        if (to.DayNumber - from.DayNumber > 3 * 366)
        {
            throw new ArgumentException("The period must not exceed three years.", nameof(to));
        }

        return _repository.StatsAsync(from, to, ct);
    }

    /// <summary>Vrai si la séance existe à cette date ; false si la cible n'existe pas ; exception si pas de séance ce jour-là.</summary>
    private async Task<bool> EnsureDateAsync(Ct.SubscriptionTargetKind kind, Guid targetId, DateOnly date, bool mondayFirst, CancellationToken ct)
    {
        var dates = await _repository.OccurrenceDatesAsync(kind, targetId, mondayFirst, ct);
        if (dates is null)
        {
            return false;
        }

        if (!dates.Contains(date))
        {
            throw new ArgumentException("There is no session on this date.", nameof(date));
        }

        return true;
    }

    /// <summary>Ateliers et événements seulement (pas de feuille d'émargement pour les projets).</summary>
    private static void EnsureKind(Ct.SubscriptionTargetKind kind)
    {
        if (kind is not (Ct.SubscriptionTargetKind.ActivitySlot or Ct.SubscriptionTargetKind.Event))
        {
            throw new ArgumentException("Attendance is recorded for activity slots and events only.", nameof(kind));
        }
    }
}
