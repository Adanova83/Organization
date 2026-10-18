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

    /// <summary>Ce qu'il faut savoir d'un créneau pour dater ses séances et titrer ses feuilles.</summary>
    private sealed record Slot(
        string Title, TimeOnly StartTime, TimeOnly EndTime, string? SpaceName,
        int DayOfWeek, int Frequency, DateOnly FirstDay, DateOnly LastDay, bool DuringTermTime, bool DuringSchoolHolidays);

    private async Task<Slot?> SlotAsync(Guid slotId, CancellationToken ct)
    {
        var slot = await _dbContext.ActivitySessionSlots
            .Where(s => s.Id == slotId)
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

        return slot is null
            ? null
            : new Slot(
                $"{slot.ActivityName} ({slot.StartTime:HH\\:mm})", slot.StartTime, slot.EndTime, slot.SpaceName,
                slot.DayOfWeek, slot.Frequency,
                DateOnly.FromDateTime(ParisTime.ToLocal(slot.Start)), DateOnly.FromDateTime(ParisTime.ToLocal(slot.End)),
                slot.DuringTermTime, slot.DuringSchoolHolidays);
    }

    public async Task<IReadOnlyList<DateOnly>?> OccurrenceDatesAsync(Guid slotId, bool mondayFirst, CancellationToken ct)
    {
        var slot = await SlotAsync(slotId, ct);
        return slot is null ? null : await DatesAsync(slot, mondayFirst, ct);
    }

    private async Task<IReadOnlyList<DateOnly>> DatesAsync(Slot slot, bool mondayFirst, CancellationToken ct)
    {
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
            slot.DayOfWeek, mondayFirst, slot.FirstDay, slot.LastDay, slot.Frequency,
            day => SessionSchedule.IsActiveOn(day, slot.DuringTermTime, slot.DuringSchoolHolidays, closed));
    }

    public async Task<Ct.AttendanceOccurrences?> OccurrencesAsync(Guid slotId, bool mondayFirst, CancellationToken ct)
    {
        var slot = await SlotAsync(slotId, ct);
        if (slot is null)
        {
            return null;
        }

        var counts = (await _dbContext.Attendances
                .Where(a => a.SessionSlotId == slotId)
                .GroupBy(a => new { a.Date, a.Status })
                .Select(g => new { g.Key.Date, g.Key.Status, Count = g.Count() })
                .ToListAsync(ct))
            .ToLookup(c => c.Date);

        int Count(DateOnly date, Ct.AttendanceStatus status) => counts[date].Where(c => c.Status == status).Sum(c => c.Count);

        // Séances du rythme, plus celles qui ont des présences hors rythme (rythme modifié après coup).
        var dates = await DatesAsync(slot, mondayFirst, ct);

        return new Ct.AttendanceOccurrences
        {
            SlotId = slotId,
            Title = slot.Title,
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

    public async Task<Ct.AttendanceSheet?> SheetAsync(Guid slotId, DateOnly date, CancellationToken ct)
    {
        var slot = await SlotAsync(slotId, ct);
        if (slot is null)
        {
            return null;
        }

        // Inscrits (hors liste d'attente) à la date de la séance : inscription faite au plus tard ce jour-là, et pas encore
        // supprimée à cette date. Les inscriptions supprimées depuis sont donc reprises (feuilles des séances passées).
        var dayEnd = ParisTime.ToUtc(date.AddDays(1).ToDateTime(TimeOnly.MinValue));
        var dayStart = ParisTime.ToUtc(date.ToDateTime(TimeOnly.MinValue));
        var subscribed = await _dbContext.ActivitySessionSlotSubscriptions.IgnoreQueryFilters()
            .Where(s => s.SessionSlotId == slotId && !s.IsWaiting && s.Audit.CreatedAt < dayEnd
                && (s.Audit.DeletedAt == null || s.Audit.DeletedAt >= dayStart))
            .Select(s => s.MemberId)
            .ToListAsync(ct);

        var records = await _dbContext.Attendances
            .Where(a => a.SessionSlotId == slotId && a.Date == date)
            .Select(a => new { a.MemberId, a.Status, a.Method, a.Comment, a.Audit.LastUpdatedAt })
            .ToListAsync(ct);
        var recordByMember = records.ToDictionary(r => r.MemberId);

        var memberIds = subscribed.Union(records.Select(r => r.MemberId)).Distinct().ToList();
        var members = await _dbContext.Members.IgnoreQueryFilters()
            .Where(m => memberIds.Contains(m.Id))
            .Select(m => new { m.Id, m.Firstname, m.Lastname, m.Birthdate })
            .ToListAsync(ct);

        var animators = await _dbContext.ActivitySessionSlotAnimations
            .Where(a => a.SessionSlotId == slotId)
            .OrderBy(a => a.Order)
            .Select(a => a.Member.Firstname + " " + a.Member.Lastname)
            .ToListAsync(ct);

        return new Ct.AttendanceSheet
        {
            SlotId = slotId,
            Date = date,
            Title = slot.Title,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            SpaceName = slot.SpaceName,
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

    public async Task RecordAsync(Guid slotId, DateOnly date, IReadOnlyCollection<Ct.AttendanceEntry> entries, Ct.AttendanceMethod method, CancellationToken ct)
    {
        var existing = await _dbContext.Attendances.Where(a => a.SessionSlotId == slotId && a.Date == date).ToListAsync(ct);

        foreach (var entry in entries)
        {
            var record = existing.FirstOrDefault(a => a.MemberId == entry.MemberId);
            if (record is null)
            {
                _dbContext.Attendances.Add(new Entity.Attendance
                {
                    Id = Guid.NewGuid(),
                    SessionSlotId = slotId,
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
            .Select(a => new { a.SessionSlotId, a.Date, a.MemberId, a.Status })
            .ToListAsync(ct);

        var slotIds = records.Select(r => r.SessionSlotId).Distinct().ToList();
        var memberIds = records.Select(r => r.MemberId).Distinct().ToList();

        // Créneaux et membres supprimés depuis : leurs présences restent comptées.
        var slotTitles = await _dbContext.ActivitySessionSlots.IgnoreQueryFilters()
            .Where(s => slotIds.Contains(s.Id))
            .Select(s => new { s.Id, s.Session.Activity.Name, s.StartTime })
            .ToDictionaryAsync(s => s.Id, ct);
        var members = await _dbContext.Members.IgnoreQueryFilters()
            .Where(m => memberIds.Contains(m.Id))
            .Select(m => new { m.Id, m.Firstname, m.Lastname })
            .ToDictionaryAsync(m => m.Id, ct);

        int Count<T>(IEnumerable<T> items, Func<T, Ct.AttendanceStatus> status, Ct.AttendanceStatus wanted)
            => items.Count(item => status(item) == wanted);

        var slots = records
            .GroupBy(r => r.SessionSlotId)
            .Select(g => new Ct.AttendanceSlotStats
            {
                SlotId = g.Key,
                Title = slotTitles.TryGetValue(g.Key, out var s) ? $"{s.Name} ({s.StartTime:HH\\:mm})" : "Créneau supprimé",
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

        return new Ct.AttendanceStats { From = from, To = to, Slots = slots, Members = memberStats };
    }
}
