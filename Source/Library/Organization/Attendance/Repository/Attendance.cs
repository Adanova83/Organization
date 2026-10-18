using System.Globalization;
using Library.Data;
using Library.Domain;
using Microsoft.EntityFrameworkCore;
using Ct = Library.Contract;

namespace Library.Repository;

public sealed class Attendance : IAttendance
{
    private static readonly CultureInfo French = CultureInfo.GetCultureInfo("fr-FR");

    private readonly FullDbContext _dbContext;

    public Attendance(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>Ce qu'il faut savoir d'un créneau ou d'un événement pour dater ses séances et titrer ses feuilles.</summary>
    private sealed record Target(
        string Title, TimeOnly StartTime, TimeOnly EndTime, string? SpaceName,
        // Créneau : jour, rythme et période ; événement : sa date.
        int DayOfWeek, int Frequency, DateOnly FirstDay, DateOnly LastDay, bool DuringTermTime, bool DuringSchoolHolidays,
        bool IsEvent);

    private async Task<Target?> TargetAsync(Ct.SubscriptionTargetKind kind, Guid targetId, CancellationToken ct)
    {
        if (kind == Ct.SubscriptionTargetKind.Event)
        {
            var item = await _dbContext.Events
                .Where(e => e.Id == targetId)
                .Select(e => new { e.Name, e.StartDate, e.EndDate, SpaceName = e.Space != null ? e.Space.Name : null })
                .FirstOrDefaultAsync(ct);
            if (item is null)
            {
                return null;
            }

            var start = ParisTime.ToLocal(item.StartDate);
            var end = ParisTime.ToLocal(item.EndDate);
            var day = DateOnly.FromDateTime(start);
            return new Target(item.Name, TimeOnly.FromDateTime(start), TimeOnly.FromDateTime(end), item.SpaceName,
                0, 1, day, day, true, true, IsEvent: true);
        }

        if (kind != Ct.SubscriptionTargetKind.ActivitySlot)
        {
            return null;
        }

        var slot = await _dbContext.ActivitySessionSlots
            .Where(s => s.Id == targetId)
            .Select(s => new
            {
                s.DayOfWeek, s.StartTime, s.EndTime, s.Frequency,
                SpaceName = s.Space != null ? s.Space.Name : null,
                ActivityName = s.Session.Activity.Name,
                Start = s.Session.StartDate ?? s.Session.SchoolYear.StartDate,
                End = s.Session.EndDate ?? s.Session.SchoolYear.EndDate,
                s.Session.DuringTermTime, s.Session.DuringSchoolHolidays
            })
            .FirstOrDefaultAsync(ct);
        if (slot is null)
        {
            return null;
        }

        return new Target(
            $"{slot.ActivityName} ({slot.StartTime:HH\\:mm})", slot.StartTime, slot.EndTime, slot.SpaceName,
            slot.DayOfWeek, slot.Frequency,
            DateOnly.FromDateTime(ParisTime.ToLocal(slot.Start)), DateOnly.FromDateTime(ParisTime.ToLocal(slot.End)),
            slot.DuringTermTime, slot.DuringSchoolHolidays, IsEvent: false);
    }

    public async Task<IReadOnlyList<DateOnly>?> OccurrenceDatesAsync(Ct.SubscriptionTargetKind kind, Guid targetId, bool mondayFirst, CancellationToken ct)
    {
        var target = await TargetAsync(kind, targetId, ct);
        return target is null ? null : await DatesAsync(target, mondayFirst, ct);
    }

    private async Task<IReadOnlyList<DateOnly>> DatesAsync(Target target, bool mondayFirst, CancellationToken ct)
    {
        if (target.IsEvent)
        {
            return [target.FirstDay];
        }

        // Vacances scolaires (rattachées à une année) et jours fériés, comme le calendrier et le flux ICS.
        var closed = (await _dbContext.Holidays
                .Select(h => new { h.StartDate, h.EndDate, h.SchoolYearId })
                .ToListAsync(ct))
            .Select(h => new ClosedPeriod(
                DateOnly.FromDateTime(ParisTime.ToLocal(h.StartDate)),
                DateOnly.FromDateTime(ParisTime.ToLocal(h.EndDate)),
                h.SchoolYearId.HasValue))
            .ToList();

        return SlotOccurrences.Active(
            target.DayOfWeek, mondayFirst, target.FirstDay, target.LastDay, target.Frequency,
            day => SessionSchedule.IsActiveOn(day, target.DuringTermTime, target.DuringSchoolHolidays, closed));
    }

    public async Task<Ct.AttendanceOccurrences?> OccurrencesAsync(Ct.SubscriptionTargetKind kind, Guid targetId, bool mondayFirst, CancellationToken ct)
    {
        var target = await TargetAsync(kind, targetId, ct);
        if (target is null)
        {
            return null;
        }

        var counts = (await Records(kind, targetId)
                .GroupBy(a => new { a.Date, a.Status })
                .Select(g => new { g.Key.Date, g.Key.Status, Count = g.Count() })
                .ToListAsync(ct))
            .ToLookup(c => c.Date);

        int Count(DateOnly date, Ct.AttendanceStatus status) => counts[date].Where(c => c.Status == status).Sum(c => c.Count);

        // Séances du rythme, plus celles qui ont des présences hors rythme (rythme modifié après coup).
        var dates = await DatesAsync(target, mondayFirst, ct);

        return new Ct.AttendanceOccurrences
        {
            Kind = kind,
            TargetId = targetId,
            Title = target.Title,
            Occurrences = dates.Union(counts.Select(c => c.Key))
                .OrderBy(date => date)
                .Select(date => new Ct.AttendanceOccurrence
                {
                    Date = date,
                    Present = Count(date, Ct.AttendanceStatus.Present),
                    Absent = Count(date, Ct.AttendanceStatus.Absent),
                    Excused = Count(date, Ct.AttendanceStatus.Excused)
                })
                .ToList()
        };
    }

    public async Task<Ct.AttendanceSheet?> SheetAsync(Ct.SubscriptionTargetKind kind, Guid targetId, DateOnly date, CancellationToken ct)
    {
        var target = await TargetAsync(kind, targetId, ct);
        if (target is null)
        {
            return null;
        }

        // Inscrits (hors liste d'attente) à la date de la séance : inscription faite au plus tard ce jour-là, et pas encore
        // supprimée à cette date. Les inscriptions supprimées depuis sont donc reprises (feuilles des séances passées).
        var dayEnd = ParisTime.ToUtc(date.AddDays(1).ToDateTime(TimeOnly.MinValue));
        var dayStart = ParisTime.ToUtc(date.ToDateTime(TimeOnly.MinValue));
        var subscribed = kind == Ct.SubscriptionTargetKind.Event
            ? await _dbContext.EventSubscriptions.IgnoreQueryFilters()
                .Where(s => s.EventId == targetId && !s.IsWaiting && s.Audit.CreatedAt < dayEnd
                    && (s.Audit.DeletedAt == null || s.Audit.DeletedAt >= dayStart))
                .Select(s => s.MemberId).ToListAsync(ct)
            : await _dbContext.ActivitySessionSlotSubscriptions.IgnoreQueryFilters()
                .Where(s => s.SessionSlotId == targetId && !s.IsWaiting && s.Audit.CreatedAt < dayEnd
                    && (s.Audit.DeletedAt == null || s.Audit.DeletedAt >= dayStart))
                .Select(s => s.MemberId).ToListAsync(ct);

        var records = await Records(kind, targetId)
            .Where(a => a.Date == date)
            .Select(a => new { a.MemberId, a.Status, a.Method, a.Comment, a.Audit.LastUpdatedAt })
            .ToListAsync(ct);
        var recordByMember = records.ToDictionary(r => r.MemberId);

        var memberIds = subscribed.Union(records.Select(r => r.MemberId)).Distinct().ToList();
        var members = await _dbContext.Members.IgnoreQueryFilters()
            .Where(m => memberIds.Contains(m.Id))
            .Select(m => new { m.Id, m.Firstname, m.Lastname, m.Birthdate })
            .ToListAsync(ct);

        var animators = kind == Ct.SubscriptionTargetKind.Event
            ? await _dbContext.EventAnimations.Where(a => a.EventId == targetId)
                .OrderBy(a => a.Order).Select(a => a.Member.Firstname + " " + a.Member.Lastname).ToListAsync(ct)
            : await _dbContext.ActivitySessionSlotAnimations.Where(a => a.SessionSlotId == targetId)
                .OrderBy(a => a.Order).Select(a => a.Member.Firstname + " " + a.Member.Lastname).ToListAsync(ct);

        return new Ct.AttendanceSheet
        {
            Kind = kind,
            TargetId = targetId,
            Date = date,
            Title = target.Title,
            StartTime = target.StartTime,
            EndTime = target.EndTime,
            SpaceName = target.SpaceName,
            Animators = animators.Select(name => name.Trim()).ToList(),
            Rows = members
                .Select(m =>
                {
                    var record = recordByMember.GetValueOrDefault(m.Id);
                    return new Ct.AttendanceRow
                    {
                        MemberId = m.Id,
                        Firstname = m.Firstname,
                        Lastname = m.Lastname,
                        Birthdate = m.Birthdate,
                        IsSubscribed = subscribed.Contains(m.Id),
                        Status = record?.Status,
                        Method = record?.Method,
                        Comment = record?.Comment,
                        RecordedAt = record?.LastUpdatedAt
                    };
                })
                .OrderBy(row => row.Lastname, StringComparer.Create(French, true))
                .ThenBy(row => row.Firstname, StringComparer.Create(French, true))
                .ToList()
        };
    }

    public async Task RecordAsync(
        Ct.SubscriptionTargetKind kind, Guid targetId, DateOnly date,
        IReadOnlyCollection<Ct.AttendanceEntry> entries, Ct.AttendanceMethod method, CancellationToken ct)
    {
        var existing = await Records(kind, targetId).Where(a => a.Date == date).ToListAsync(ct);

        foreach (var entry in entries)
        {
            var record = existing.FirstOrDefault(a => a.MemberId == entry.MemberId);
            if (record is null)
            {
                _dbContext.Attendances.Add(new Entity.Attendance
                {
                    Id = Guid.NewGuid(),
                    SessionSlotId = kind == Ct.SubscriptionTargetKind.ActivitySlot ? targetId : null,
                    EventId = kind == Ct.SubscriptionTargetKind.Event ? targetId : null,
                    Date = date,
                    MemberId = entry.MemberId,
                    Status = entry.Status,
                    Method = method,
                    Active = new Entity.Active(),
                    Audit = new Entity.Audit()
                });
            }
            else if (record.Status != entry.Status)
            {
                record.Status = entry.Status;
                record.Method = method;
            }
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<Ct.AttendanceStats> StatsAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        var records = await _dbContext.Attendances
            .Where(a => a.Date >= from && a.Date <= to)
            .Select(a => new { a.SessionSlotId, a.EventId, a.Date, a.MemberId, a.Status })
            .ToListAsync(ct);

        var slotIds = records.Where(r => r.SessionSlotId.HasValue).Select(r => r.SessionSlotId!.Value).Distinct().ToList();
        var eventIds = records.Where(r => r.EventId.HasValue).Select(r => r.EventId!.Value).Distinct().ToList();
        var memberIds = records.Select(r => r.MemberId).Distinct().ToList();

        var slotTitles = await _dbContext.ActivitySessionSlots.IgnoreQueryFilters()
            .Where(s => slotIds.Contains(s.Id))
            .Select(s => new { s.Id, s.Session.Activity.Name, s.StartTime, s.DayOfWeek })
            .ToDictionaryAsync(s => s.Id, ct);
        var eventTitles = await _dbContext.Events.IgnoreQueryFilters()
            .Where(e => eventIds.Contains(e.Id))
            .Select(e => new { e.Id, e.Name, e.StartDate })
            .ToDictionaryAsync(e => e.Id, ct);
        var members = await _dbContext.Members.IgnoreQueryFilters()
            .Where(m => memberIds.Contains(m.Id))
            .Select(m => new { m.Id, m.Firstname, m.Lastname })
            .ToDictionaryAsync(m => m.Id, ct);

        int Count<T>(IEnumerable<T> items, Func<T, Ct.AttendanceStatus> status, Ct.AttendanceStatus wanted)
            => items.Count(item => status(item) == wanted);

        var targets = records
            .GroupBy(r => (Kind: r.EventId.HasValue ? Ct.SubscriptionTargetKind.Event : Ct.SubscriptionTargetKind.ActivitySlot,
                Id: r.EventId ?? r.SessionSlotId!.Value))
            .Select(g => new Ct.AttendanceTargetStats
            {
                Kind = g.Key.Kind,
                TargetId = g.Key.Id,
                Title = g.Key.Kind == Ct.SubscriptionTargetKind.Event
                    ? eventTitles.TryGetValue(g.Key.Id, out var e) ? $"{e.Name} ({ParisTime.ToLocal(e.StartDate):dd/MM/yyyy})" : "Événement supprimé"
                    : slotTitles.TryGetValue(g.Key.Id, out var s) ? $"{s.Name} ({s.StartTime:HH\\:mm})" : "Créneau supprimé",
                Sessions = g.Select(r => r.Date).Distinct().Count(),
                Present = Count(g, r => r.Status, Ct.AttendanceStatus.Present),
                Absent = Count(g, r => r.Status, Ct.AttendanceStatus.Absent),
                Excused = Count(g, r => r.Status, Ct.AttendanceStatus.Excused)
            })
            .OrderBy(t => t.Title, StringComparer.Create(French, true))
            .ToList();

        var memberStats = records
            .GroupBy(r => r.MemberId)
            .Select(g => new Ct.AttendanceMemberStats
            {
                MemberId = g.Key,
                Firstname = members.GetValueOrDefault(g.Key)?.Firstname,
                Lastname = members.GetValueOrDefault(g.Key)?.Lastname ?? "Membre supprimé",
                Present = Count(g, r => r.Status, Ct.AttendanceStatus.Present),
                Absent = Count(g, r => r.Status, Ct.AttendanceStatus.Absent),
                Excused = Count(g, r => r.Status, Ct.AttendanceStatus.Excused)
            })
            .OrderBy(m => m.Lastname, StringComparer.Create(French, true))
            .ThenBy(m => m.Firstname, StringComparer.Create(French, true))
            .ToList();

        return new Ct.AttendanceStats { From = from, To = to, Targets = targets, Members = memberStats };
    }

    private IQueryable<Entity.Attendance> Records(Ct.SubscriptionTargetKind kind, Guid targetId)
        => kind == Ct.SubscriptionTargetKind.Event
            ? _dbContext.Attendances.Where(a => a.EventId == targetId)
            : _dbContext.Attendances.Where(a => a.SessionSlotId == targetId);
}
