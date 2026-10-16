using Microsoft.Extensions.Options;
using D = Library.Domain;
using R = Library.Repository;

namespace Api.Calendar;

/// <summary>
/// Construit les flux iCalendar du planning (événements, créneaux d'activités, créneaux de projets, jours fériés).
/// Le planning reste la donnée de cette application ; Nextcloud ne fait que s'abonner en lecture.
/// </summary>
public sealed class CalendarFeedService
{
    public const string Events = "events";
    public const string Activities = "activities";
    public const string Projects = "projects";
    public const string Holidays = "holidays";
    public const string All = "all";

    private readonly CalendarFeedOptions _options;
    private readonly R.IEvent _events;
    private readonly R.IActivity _activities;
    private readonly R.IActivitySession _sessions;
    private readonly R.IActivitySessionSlot _activitySlots;
    private readonly R.IProject _projects;
    private readonly R.IProjectSlot _projectSlots;
    private readonly R.IHoliday _holidays;
    private readonly R.ISpace _spaces;
    private readonly ILogger<CalendarFeedService> _logger;

    public CalendarFeedService(
        IOptions<CalendarFeedOptions> options,
        R.IEvent events,
        R.IActivity activities,
        R.IActivitySession sessions,
        R.IActivitySessionSlot activitySlots,
        R.IProject projects,
        R.IProjectSlot projectSlots,
        R.IHoliday holidays,
        R.ISpace spaces,
        ILogger<CalendarFeedService> logger)
    {
        _options = options.Value;
        _events = events;
        _activities = activities;
        _sessions = sessions;
        _activitySlots = activitySlots;
        _projects = projects;
        _projectSlots = projectSlots;
        _holidays = holidays;
        _spaces = spaces;
        _logger = logger;
    }

    /// <summary>Retourne le flux demandé, ou null si le nom du flux est inconnu.</summary>
    public async Task<string?> BuildAsync(string feed, CancellationToken ct)
    {
        feed = feed.ToLowerInvariant();

        if (feed is not (Events or Activities or Projects or Holidays or All))
        {
            return null;
        }

        var spaceList = await _spaces.SearchAsync(null, 0, 0, ct);
        var spaces = spaceList.Data.ToDictionary(space => space.Id);

        var items = new List<IcsEvent>();

        if (feed is Events or All)
        {
            items.AddRange(await LoadEventsAsync(spaces, ct));
        }

        if (feed is Activities or All)
        {
            items.AddRange(await LoadActivitySlotsAsync(spaces, ct));
        }

        if (feed is Projects or All)
        {
            items.AddRange(await LoadProjectSlotsAsync(spaces, ct));
        }

        if (feed is Holidays or All)
        {
            items.AddRange(await LoadHolidaysAsync(ct));
        }

        return IcsBuilder.Calendar(Title(feed), items);
    }

    private static string Title(string feed) => feed switch
    {
        Events => "Organisation – Événements",
        Activities => "Organisation – Activités",
        Projects => "Organisation – Projets",
        Holidays => "Organisation – Jours fériés",
        _ => "Organisation – Planning"
    };

    private async Task<List<IcsEvent>> LoadEventsAsync(IReadOnlyDictionary<Guid, D.Space> spaces, CancellationToken ct)
    {
        var items = new List<IcsEvent>();
        var response = await _events.SearchAsync(null, 0, 0, ct);

        foreach (var item in response.Data)
        {
            var space = FindSpace(spaces, item.SpaceId);
            var changed = Latest(item.Audit?.LastUpdatedAt, space?.Audit?.LastUpdatedAt);
            items.Add(IcsBuilder.OneOff(
                Uid(nameof(D.Event), item.Id), changed, item.Name, Describe(item.Description, item.AgeMinimum), space?.Name,
                item.StartDate, item.EndDate, "Événement"));
        }

        return items;
    }

    private async Task<List<IcsEvent>> LoadActivitySlotsAsync(IReadOnlyDictionary<Guid, D.Space> spaces, CancellationToken ct)
    {
        var items = new List<IcsEvent>();
        var activities = await _activities.SearchAsync(null, 0, 0, ct);

        // Vacances scolaires (rattachées à une année) et jours fériés : appliqués selon le rythme de chaque session.
        var closedPeriods = (await _holidays.SearchAsync(null, 0, 0, ct)).Data
            .Select(h => new D.ClosedPeriod(
                DateOnly.FromDateTime(ParisTime.ToLocal(h.StartDate)),
                DateOnly.FromDateTime(ParisTime.ToLocal(h.EndDate)),
                h.SchoolYearId.HasValue))
            .ToList();

        foreach (var activity in activities.Data)
        {
            var sessions = await _sessions.SearchAsync(activity.Id, 0, 0, ct);

            foreach (var session in sessions.Data)
            {
                // Dates effectives : celles de la session, sinon celles de son année scolaire.
                if (session.EffectiveStartDate is not { } sessionStart || session.EffectiveEndDate is not { } sessionEnd)
                {
                    continue;
                }

                var slots = await _activitySlots.SearchAsync(session.Id, 0, 0, ct);

                foreach (var slot in slots.Data)
                {
                    var space = FindSpace(spaces, slot.SpaceId);
                    var changed = Latest(
                        slot.Audit?.LastUpdatedAt,
                        session.Audit?.LastUpdatedAt,
                        activity.Audit?.LastUpdatedAt,
                        space?.Audit?.LastUpdatedAt);
                    var ics = IcsBuilder.Weekly(
                        Uid(nameof(D.ActivitySessionSlot), slot.Id), changed, activity.Name,
                        Describe(activity.Description, activity.AgeMinimum), space?.Name,
                        slot.DayOfWeek, _options.DayOfWeekStartsOnMonday, sessionStart, sessionEnd,
                        slot.StartTime, slot.EndTime, slot.Frequency, "Activité",
                        day => D.SessionSchedule.IsActiveOn(day, session.DuringTermTime, session.DuringSchoolHolidays, closedPeriods));

                    if (ics is not null)
                    {
                        items.Add(ics);
                    }
                }
            }
        }

        return items;
    }

    private async Task<List<IcsEvent>> LoadProjectSlotsAsync(IReadOnlyDictionary<Guid, D.Space> spaces, CancellationToken ct)
    {
        var items = new List<IcsEvent>();
        var projects = await _projects.SearchAsync(null, 0, 0, ct);

        foreach (var project in projects.Data)
        {
            // Dates effectives : celles du projet, sinon celles de son année scolaire.
            if (project.EffectiveStartDate is not { } projectStart || project.EffectiveEndDate is not { } projectEnd || projectEnd < projectStart)
            {
                _logger.LogWarning("Project {Name}: missing or invalid dates, slots ignored in the calendar feed.", project.Name);
                continue;
            }

            var slots = await _projectSlots.SearchAsync(project.Id, 0, 0, ct);

            foreach (var slot in slots.Data)
            {
                var space = FindSpace(spaces, slot.SpaceId);
                var changed = Latest(
                    slot.Audit?.LastUpdatedAt,
                    project.Audit?.LastUpdatedAt,
                    space?.Audit?.LastUpdatedAt);
                var ics = IcsBuilder.Weekly(
                    Uid(nameof(D.ProjectSlot), slot.Id), changed, project.Name,
                    Describe(project.Description, project.AgeMinimum), space?.Name,
                    slot.DayOfWeek, _options.DayOfWeekStartsOnMonday, projectStart, projectEnd,
                    slot.StartTime, slot.EndTime, 1, "Projet");

                if (ics is not null)
                {
                    items.Add(ics);
                }
            }
        }

        return items;
    }

    private async Task<List<IcsEvent>> LoadHolidaysAsync(CancellationToken ct)
    {
        var items = new List<IcsEvent>();
        var response = await _holidays.SearchAsync(null, 0, 0, ct);

        foreach (var item in response.Data)
        {
            var changed = Latest(item.Audit?.LastUpdatedAt);
            var firstDay = DateOnly.FromDateTime(ParisTime.ToLocal(item.StartDate));
            var lastDay = DateOnly.FromDateTime(ParisTime.ToLocal(item.EndDate));
            items.Add(IcsBuilder.AllDay(Uid(nameof(D.Holiday), item.Id), changed, item.Name, null, firstDay, lastDay, "Jour férié"));
        }

        return items;
    }

    private string Uid(string entityType, Guid id)
        => $"{entityType.ToLowerInvariant()}-{id:N}@{_options.UidDomain}";

    private static D.Space? FindSpace(IReadOnlyDictionary<Guid, D.Space> spaces, Guid? id)
        => id.HasValue && spaces.TryGetValue(id.Value, out var space) ? space : null;

    private static string? Describe(string? description, int? ageMinimum)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(description))
        {
            parts.Add(description.Trim());
        }

        if (ageMinimum is > 0)
        {
            parts.Add($"Âge minimum : {ageMinimum} ans");
        }

        return parts.Count == 0 ? null : string.Join("\n", parts);
    }

    /// <summary>Date de modification la plus récente. Sans information, « maintenant ».</summary>
    private static DateTime Latest(params DateTime?[] values)
    {
        DateTime? latest = null;

        foreach (var value in values)
        {
            if (value.HasValue && (!latest.HasValue || value.Value > latest.Value))
            {
                latest = value;
            }
        }

        return latest.HasValue ? ParisTime.AsUtc(latest.Value) : DateTime.UtcNow;
    }
}
