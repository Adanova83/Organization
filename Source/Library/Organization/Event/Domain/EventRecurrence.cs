namespace Library.Domain;

public enum RecurrenceFrequency
{
    /// <summary>Tous les N jours.</summary>
    Daily,
    /// <summary>Toutes les N semaines, un jour de la semaine donné.</summary>
    Weekly,
    /// <summary>Tous les N mois, un numéro de jour donné (mois trop courts : dernier jour du mois).</summary>
    MonthlyByDate,
    /// <summary>Tous les N mois, un jour de la semaine à un rang donné (2e mardi, dernier vendredi...).</summary>
    MonthlyByWeekday
}

/// <summary>Dates des occurrences d'une série d'événements sur une période (bornes incluses).</summary>
public static class EventRecurrence
{
    public const int MaxOccurrences = 200;
    public const int MaxYears = 2;

    /// <summary>Rang « dernier » du jour dans le mois (MonthlyByWeekday).</summary>
    public const int LastWeekOfMonth = -1;

    /// <param name="dayOfWeek">Weekly et MonthlyByWeekday : jour de la semaine.</param>
    /// <param name="dayOfMonth">MonthlyByDate : numéro du jour (1 à 31).</param>
    /// <param name="weekOfMonth">MonthlyByWeekday : rang du jour dans le mois (1 à 4, ou <see cref="LastWeekOfMonth"/>).</param>
    public static IReadOnlyList<DateOnly> Dates(
        DateOnly from, DateOnly until, RecurrenceFrequency frequency, int interval,
        DayOfWeek? dayOfWeek = null, int? dayOfMonth = null, int? weekOfMonth = null)
    {
        if (interval is < 1 or > 52)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be between 1 and 52.");
        }

        if (until < from)
        {
            throw new ArgumentOutOfRangeException(nameof(until), "The end of the recurrence must follow its start.");
        }

        if (until > from.AddYears(MaxYears))
        {
            throw new ArgumentOutOfRangeException(nameof(until), $"A recurrence cannot last more than {MaxYears} years.");
        }

        IEnumerable<DateOnly> candidates = frequency switch
        {
            RecurrenceFrequency.Daily => Every(from, interval),
            RecurrenceFrequency.Weekly => Every(NextOrSame(from, Required(dayOfWeek, nameof(dayOfWeek))), 7 * interval),
            RecurrenceFrequency.MonthlyByDate => Months(from, interval, month => SameDay(month, RequiredDayOfMonth(dayOfMonth))),
            RecurrenceFrequency.MonthlyByWeekday => Months(from, interval,
                month => Ranked(month, Required(dayOfWeek, nameof(dayOfWeek)), RequiredWeekOfMonth(weekOfMonth))),
            _ => throw new ArgumentOutOfRangeException(nameof(frequency))
        };

        var dates = new List<DateOnly>();
        foreach (var date in candidates.Where(date => date >= from))
        {
            if (date > until)
            {
                break;
            }

            dates.Add(date);
            if (dates.Count > MaxOccurrences)
            {
                throw new ArgumentOutOfRangeException(nameof(until), $"A recurrence cannot have more than {MaxOccurrences} occurrences.");
            }
        }

        return dates;
    }

    private static IEnumerable<DateOnly> Every(DateOnly first, int days)
    {
        for (var date = first; ; date = date.AddDays(days))
        {
            yield return date;
        }
    }

    /// <summary>Une date par mois retenu (le mois de <paramref name="from"/>, puis tous les <paramref name="interval"/> mois).</summary>
    private static IEnumerable<DateOnly> Months(DateOnly from, int interval, Func<DateOnly, DateOnly> dateInMonth)
    {
        for (var month = new DateOnly(from.Year, from.Month, 1); ; month = month.AddMonths(interval))
        {
            yield return dateInMonth(month);
        }
    }

    private static DateOnly NextOrSame(DateOnly date, DayOfWeek day) => date.AddDays(((int)day - (int)date.DayOfWeek + 7) % 7);

    private static DateOnly SameDay(DateOnly month, int dayOfMonth)
        => month.AddDays(Math.Min(dayOfMonth, DateTime.DaysInMonth(month.Year, month.Month)) - 1);

    private static DateOnly Ranked(DateOnly month, DayOfWeek day, int weekOfMonth)
    {
        if (weekOfMonth == LastWeekOfMonth)
        {
            var last = month.AddDays(DateTime.DaysInMonth(month.Year, month.Month) - 1);
            return last.AddDays(-(((int)last.DayOfWeek - (int)day + 7) % 7));
        }

        return NextOrSame(month, day).AddDays(7 * (weekOfMonth - 1));
    }

    private static DayOfWeek Required(DayOfWeek? day, string name)
        => day ?? throw new ArgumentException("The day of the week is required for this frequency.", name);

    private static int RequiredDayOfMonth(int? day) => day is >= 1 and <= 31
        ? day.Value
        : throw new ArgumentOutOfRangeException(nameof(day), "The day of the month must be between 1 and 31.");

    private static int RequiredWeekOfMonth(int? week) => week is (>= 1 and <= 4) or LastWeekOfMonth
        ? week.Value
        : throw new ArgumentOutOfRangeException(nameof(week), "The week of the month must be 1 to 4, or -1 for the last one.");
}
