namespace Api.Calendar;

/// <summary>
/// Conversions UTC / heure de Paris sans dépendre de la base de fuseaux horaires du système
/// (absente de certaines images Docker). Règle européenne : heure d'été du dernier dimanche de mars
/// au dernier dimanche d'octobre, à 01:00 UTC.
/// </summary>
public static class ParisTime
{
    public static DateTime AsUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Local => value.ToUniversalTime(),
        DateTimeKind.Utc => value,
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    /// <summary>Heure murale de Paris (Kind = Unspecified) correspondant à un instant UTC.</summary>
    public static DateTime ToLocal(DateTime value)
    {
        var utc = AsUtc(value);
        return DateTime.SpecifyKind(utc + Offset(utc), DateTimeKind.Unspecified);
    }

    /// <summary>Instant UTC correspondant à une heure murale de Paris.</summary>
    public static DateTime ToUtc(DateTime local)
    {
        var wallClock = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);

        var summer = DateTime.SpecifyKind(wallClock - TimeSpan.FromHours(2), DateTimeKind.Utc);
        if (Offset(summer) == TimeSpan.FromHours(2))
        {
            return summer;
        }

        return DateTime.SpecifyKind(wallClock - TimeSpan.FromHours(1), DateTimeKind.Utc);
    }

    private static TimeSpan Offset(DateTime utc)
    {
        var start = LastSunday(utc.Year, 3).AddHours(1);
        var end = LastSunday(utc.Year, 10).AddHours(1);
        return utc >= start && utc < end ? TimeSpan.FromHours(2) : TimeSpan.FromHours(1);
    }

    private static DateTime LastSunday(int year, int month)
    {
        var last = new DateTime(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, DateTimeKind.Utc);
        return last.AddDays(-(int)last.DayOfWeek);
    }
}
