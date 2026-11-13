using Library.Data;
using Library.Domain;
using Microsoft.EntityFrameworkCore;
using Ct = Library.Contract;

namespace Library.Repository;

public sealed class Agenda : IAgenda
{
    private readonly FullDbContext _dbContext;

    public Agenda(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>Créneau hebdomadaire (atelier ou projet) et ce qu'il faut pour dater ses séances.</summary>
    private sealed record WeeklySlot(
        Guid Id, Guid ParentId, string Title, int DayOfWeek, TimeOnly StartTime, TimeOnly EndTime, int Frequency,
        string? SpaceName, DateTime Start, DateTime End, bool DuringTermTime, bool DuringSchoolHolidays, DateOnly? Date = null);

    public async Task<IReadOnlyList<Ct.AgendaEntry>> ListAsync(Guid memberId, DateOnly from, DateOnly to, bool mondayFirst, CancellationToken ct)
    {
        var entries = new List<Ct.AgendaEntry>();

        // Rôle par créneau : animateur l'emporte sur participant ; en attente seulement si participant en attente.
        // Requêtes l'une après l'autre : un contexte EF Core n'en accepte qu'une à la fois.
        var activityRoles = Roles(
            await _dbContext.ActivitySessionSlotSubscriptions.Where(s => s.MemberId == memberId)
                .Select(s => new Subscribed(s.SessionSlotId, s.IsWaiting)).ToListAsync(ct),
            await _dbContext.ActivitySessionSlotAnimations.Where(a => a.MemberId == memberId).Select(a => a.SessionSlotId).ToListAsync(ct));
        var projectRoles = Roles(
            await _dbContext.ProjectSlotSubscriptions.Where(s => s.MemberId == memberId)
                .Select(s => new Subscribed(s.ProjectSlotId, s.IsWaiting)).ToListAsync(ct),
            await _dbContext.ProjectSlotAnimations.Where(a => a.MemberId == memberId).Select(a => a.ProjectSlotId).ToListAsync(ct));

        if (activityRoles.Count + projectRoles.Count > 0)
        {
            var closed = await ClosedPeriodsAsync(ct);

            var activityIds = activityRoles.Keys.ToList();
            var activitySlots = await _dbContext.ActivitySessionSlots
                .Where(s => activityIds.Contains(s.Id))
                .Select(s => new WeeklySlot(
                    s.Id, s.Session.ActivityId, s.Session.Activity.Name, s.DayOfWeek, s.StartTime, s.EndTime, s.Frequency,
                    s.Space != null ? s.Space.Name : null,
                    s.Session.StartDate ?? s.Session.SchoolYear.StartDate, s.Session.EndDate ?? s.Session.SchoolYear.EndDate,
                    s.Session.DuringTermTime, s.Session.DuringSchoolHolidays))
                .ToListAsync(ct);
            entries.AddRange(Expand(activitySlots, Ct.SubscriptionTargetKind.ActivitySlot, activityRoles, from, to, mondayFirst, closed));

            var projectIds = projectRoles.Keys.ToList();
            var projectSlots = await _dbContext.ProjectSlots
                .Where(s => projectIds.Contains(s.Id))
                .Select(s => new WeeklySlot(
                    s.Id, s.ProjectId, s.Project.Name, s.DayOfWeek, s.StartTime, s.EndTime, 1,
                    s.Space != null ? s.Space.Name : null,
                    s.Project.StartDate ?? s.Project.SchoolYear.StartDate, s.Project.EndDate ?? s.Project.SchoolYear.EndDate,
                    s.Project.DuringTermTime, s.Project.DuringSchoolHolidays, s.Date))
                .ToListAsync(ct);
            entries.AddRange(Expand(projectSlots, Ct.SubscriptionTargetKind.ProjectSlot, projectRoles, from, to, mondayFirst, closed));
        }

        // Événements (pas d'animateurs) : ceux auxquels le membre est inscrit, dans la période (heure de Paris).
        var rangeStart = ParisTime.ToUtc(from.ToDateTime(TimeOnly.MinValue));
        var rangeEnd = ParisTime.ToUtc(to.AddDays(1).ToDateTime(TimeOnly.MinValue));
        var events = await _dbContext.EventSubscriptions
            .Where(s => s.MemberId == memberId && s.Event.StartDate >= rangeStart && s.Event.StartDate < rangeEnd)
            .Select(s => new { s.Event.Id, s.Event.Name, s.Event.StartDate, s.Event.EndDate, SpaceName = s.Event.Space != null ? s.Event.Space.Name : null, s.IsWaiting })
            .ToListAsync(ct);
        entries.AddRange(events.Select(e =>
        {
            var start = ParisTime.ToLocal(e.StartDate);
            return new Ct.AgendaEntry
            {
                Kind = Ct.SubscriptionTargetKind.Event,
                TargetId = e.Id,
                ParentId = e.Id,
                Date = DateOnly.FromDateTime(start),
                StartTime = TimeOnly.FromDateTime(start),
                EndTime = TimeOnly.FromDateTime(ParisTime.ToLocal(e.EndDate)),
                Title = e.Name,
                SpaceName = e.SpaceName,
                Role = Ct.AgendaRole.Participant,
                IsWaiting = e.IsWaiting
            };
        }));

        return entries.OrderBy(e => e.Date).ThenBy(e => e.StartTime).ThenBy(e => e.Title).ToList();
    }

    private sealed record Subscribed(Guid SlotId, bool IsWaiting);

    private static Dictionary<Guid, (Ct.AgendaRole Role, bool IsWaiting)> Roles(List<Subscribed> subscriptions, List<Guid> animations)
    {
        var roles = new Dictionary<Guid, (Ct.AgendaRole, bool)>();
        foreach (var subscription in subscriptions)
        {
            roles[subscription.SlotId] = (Ct.AgendaRole.Participant, subscription.IsWaiting);
        }

        foreach (var id in animations)
        {
            roles[id] = (Ct.AgendaRole.Animator, false);
        }

        return roles;
    }

    private static IEnumerable<Ct.AgendaEntry> Expand(
        IEnumerable<WeeklySlot> slots, Ct.SubscriptionTargetKind kind, IReadOnlyDictionary<Guid, (Ct.AgendaRole Role, bool IsWaiting)> roles,
        DateOnly from, DateOnly to, bool mondayFirst, IReadOnlyCollection<ClosedPeriod> closed)
    {
        foreach (var slot in slots)
        {
            // Séances du rythme, limitées à la période demandée.
            var first = DateOnly.FromDateTime(ParisTime.ToLocal(slot.Start));
            var last = DateOnly.FromDateTime(ParisTime.ToLocal(slot.End));
            // Créneau ponctuel (projet) : sa date seulement, sans rythme ni période.
            IEnumerable<DateOnly> dates = slot.Date is { } single
                ? (single >= from && single <= to ? [single] : [])
                : SlotOccurrences.Active(
                    slot.DayOfWeek, mondayFirst, first, last, slot.Frequency,
                    day => day >= from && day <= to && SessionSchedule.IsActiveOn(day, slot.DuringTermTime, slot.DuringSchoolHolidays, closed));

            var (role, isWaiting) = roles[slot.Id];
            foreach (var date in dates)
            {
                yield return new Ct.AgendaEntry
                {
                    Kind = kind,
                    TargetId = slot.Id,
                    ParentId = slot.ParentId,
                    Date = date,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    Title = slot.Title,
                    SpaceName = slot.SpaceName,
                    Role = role,
                    IsWaiting = isWaiting
                };
            }
        }
    }

    /// <summary>Vacances scolaires (rattachées à une année) et jours fériés, comme le calendrier et le flux ICS.</summary>
    private async Task<List<ClosedPeriod>> ClosedPeriodsAsync(CancellationToken ct)
        => (await _dbContext.Holidays.Select(h => new { h.StartDate, h.EndDate, h.SchoolYearId }).ToListAsync(ct))
            .Select(h => new ClosedPeriod(
                DateOnly.FromDateTime(ParisTime.ToLocal(h.StartDate)),
                DateOnly.FromDateTime(ParisTime.ToLocal(h.EndDate)),
                h.SchoolYearId.HasValue))
            .ToList();
}
