namespace Library.Domain;

/// <summary>
/// Dates des séances d'un créneau hebdomadaire : le jour du créneau, toutes les <c>intervalWeeks</c> semaines, entre le
/// premier et le dernier jour de la période, sans les jours exclus par <see cref="SessionSchedule"/> (vacances, fériés).
/// Mêmes règles que le flux ICS (RRULE + EXDATE).
/// </summary>
public static class SlotOccurrences
{
    /// <summary>Jour de la semaine d'un créneau : 0 = dimanche, ou 0 = lundi si <paramref name="mondayFirst"/>.</summary>
    public static DayOfWeek ToDayOfWeek(int value, bool mondayFirst)
        => mondayFirst ? (DayOfWeek)((value + 1) % 7) : (DayOfWeek)(value % 7);

    /// <summary>Première date du créneau à partir de <paramref name="firstDay"/>.</summary>
    public static DateOnly First(int dayOfWeek, bool mondayFirst, DateOnly firstDay)
    {
        var target = ToDayOfWeek(dayOfWeek, mondayFirst);
        return firstDay.AddDays(((int)target - (int)firstDay.DayOfWeek + 7) % 7);
    }

    /// <summary>Toutes les dates du rythme (avant exclusions), de la première au dernier jour inclus.</summary>
    public static IReadOnlyList<DateOnly> All(int dayOfWeek, bool mondayFirst, DateOnly firstDay, DateOnly lastDay, int intervalWeeks)
    {
        var first = First(dayOfWeek, mondayFirst, firstDay);
        if (first > lastDay)
        {
            return [];
        }

        var step = 7 * Math.Max(intervalWeeks, 1);
        return Enumerable.Range(0, (lastDay.DayNumber - first.DayNumber) / step + 1).Select(i => first.AddDays(i * step)).ToList();
    }

    /// <summary>Dates où le créneau a réellement lieu.</summary>
    public static IReadOnlyList<DateOnly> Active(
        int dayOfWeek, bool mondayFirst, DateOnly firstDay, DateOnly lastDay, int intervalWeeks, Func<DateOnly, bool> isActive)
        => All(dayOfWeek, mondayFirst, firstDay, lastDay, intervalWeeks).Where(isActive).ToList();
}
