namespace Library.Contract;

public enum RecurrenceFrequency
{
    Daily,
    Weekly,
    MonthlyByDate,
    MonthlyByWeekday
}

/// <summary>
/// Création d'une série d'événements (POST /api/events/series). <see cref="Event"/> donne les champs communs
/// (nom, code, organisation, année scolaire, lieu...) ; ses dates sont ignorées. Une occurrence est créée à chaque date
/// de la récurrence sur la période de l'année scolaire (à partir d'aujourd'hui si l'année a commencé), de
/// <see cref="StartTime"/> à <see cref="EndTime"/> (heure de Paris ; une fin avant le début : le lendemain).
/// </summary>
public class EventSeries
{
    public Event Event { get; set; } = default!;
    public RecurrenceFrequency Frequency { get; set; } = RecurrenceFrequency.Weekly;

    /// <summary>Tous les N jours, semaines ou mois selon la fréquence.</summary>
    public int Interval { get; set; } = 1;

    /// <summary>Weekly et MonthlyByWeekday : jour de la semaine, 0 = dimanche … 6 = samedi.</summary>
    public int? DayOfWeek { get; set; }

    /// <summary>MonthlyByDate : numéro du jour du mois (1 à 31 ; mois plus court : dernier jour).</summary>
    public int? DayOfMonth { get; set; }

    /// <summary>MonthlyByWeekday : rang du jour dans le mois, 1 à 4, ou -1 pour le dernier.</summary>
    public int? WeekOfMonth { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    // Rythme, comme les sessions d'activité : hors vacances scolaires et/ou pendant les vacances ; jamais les jours fériés.
    public bool DuringTermTime { get; set; } = true;
    public bool DuringSchoolHolidays { get; set; }

    /// <summary>Facultatives : par défaut, la période de l'année scolaire.</summary>
    public DateOnly? From { get; set; }
    public DateOnly? Until { get; set; }
}

/// <summary>
/// Modification commune des occurrences d'une série (PUT /api/events/series/{id}?from=) : chaque occurrence garde sa date ;
/// l'heure de début (heure de Paris) et la durée s'appliquent à toutes.
/// </summary>
public class EventSeriesUpdate
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int? AgeMinimum { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid SchoolYearId { get; set; }
    public Guid? SpaceId { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationMinutes { get; set; }
}
