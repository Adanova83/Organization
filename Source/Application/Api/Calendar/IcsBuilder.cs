using System.Globalization;
using System.Text;

namespace Api.Calendar;

/// <summary>Un VEVENT iCalendar sérialisé, prêt à être inséré dans un flux.</summary>
public sealed record IcsEvent(string Text, bool UsesTimeZone);

/// <summary>Génération d'événements et de flux iCalendar (RFC 5545), auxquels Nextcloud peut s'abonner.</summary>
public static class IcsBuilder
{
    private const string ProductId = "-//Organizations//Flux ICS//FR";
    private const string TimeZoneId = "Europe/Paris";

    // Définition standard de Europe/Paris, nécessaire pour les événements récurrents en heure locale.
    private static readonly string[] VTimeZone =
    {
        "BEGIN:VTIMEZONE",
        "TZID:" + TimeZoneId,
        "BEGIN:DAYLIGHT",
        "TZOFFSETFROM:+0100",
        "TZOFFSETTO:+0200",
        "TZNAME:CEST",
        "DTSTART:19700329T020000",
        "RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=-1SU",
        "END:DAYLIGHT",
        "BEGIN:STANDARD",
        "TZOFFSETFROM:+0200",
        "TZOFFSETTO:+0100",
        "TZNAME:CET",
        "DTSTART:19701025T030000",
        "RRULE:FREQ=YEARLY;BYMONTH=10;BYDAY=-1SU",
        "END:STANDARD",
        "END:VTIMEZONE"
    };

    /// <summary>Événement ponctuel, défini par deux instants UTC.</summary>
    public static IcsEvent OneOff(
        string uid, DateTime changedUtc, string summary, string? description, string? location,
        DateTime startUtc, DateTime endUtc, string? category)
    {
        var start = ParisTime.AsUtc(startUtc);
        var end = ParisTime.AsUtc(endUtc);
        if (end < start)
        {
            end = start;
        }

        var lines = new List<string>();
        AddCommon(lines, uid, changedUtc, summary, description, location, category);
        lines.Add("DTSTART:" + FormatUtc(start));
        lines.Add("DTEND:" + FormatUtc(end));
        return Finish(lines, usesTimeZone: false);
    }

    /// <summary>Événement sur une ou plusieurs journées entières (dernier jour inclus).</summary>
    public static IcsEvent AllDay(
        string uid, DateTime changedUtc, string summary, string? description,
        DateOnly firstDay, DateOnly lastDay, string? category)
    {
        if (lastDay < firstDay)
        {
            lastDay = firstDay;
        }

        var lines = new List<string>();
        AddCommon(lines, uid, changedUtc, summary, description, null, category);
        lines.Add("DTSTART;VALUE=DATE:" + firstDay.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        lines.Add("DTEND;VALUE=DATE:" + lastDay.AddDays(1).ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        lines.Add("TRANSP:TRANSPARENT");
        return Finish(lines, usesTimeZone: false);
    }

    /// <summary>
    /// Créneau récurrent (toutes les <paramref name="intervalWeeks"/> semaines) en heure de Paris, entre deux dates (bornes incluses).
    /// Retourne null si aucune occurrence ne tombe dans la période.
    /// </summary>
    public static IcsEvent? Weekly(
        string uid, DateTime changedUtc, string summary, string? description, string? location,
        int dayOfWeek, bool mondayFirst, DateTime rangeStartUtc, DateTime rangeEndUtc,
        TimeOnly start, TimeOnly end, int intervalWeeks, string? category, Func<DateOnly, bool>? isActive = null)
    {
        var target = Library.Domain.SlotOccurrences.ToDayOfWeek(dayOfWeek, mondayFirst);
        var firstDay = DateOnly.FromDateTime(ParisTime.ToLocal(rangeStartUtc));
        var lastDay = DateOnly.FromDateTime(ParisTime.ToLocal(rangeEndUtc));

        var first = Library.Domain.SlotOccurrences.First(dayOfWeek, mondayFirst, firstDay);
        if (first > lastDay)
        {
            return null;
        }

        // UNTIL est inclusif : l'occurrence du dernier jour, qui commence à l'heure de début, est conservée.
        var until = ParisTime.ToUtc(lastDay.ToDateTime(start));

        var lines = new List<string>();
        AddCommon(lines, uid, changedUtc, summary, description, location, category);
        lines.Add($"DTSTART;TZID={TimeZoneId}:{FormatLocal(first, start)}");
        lines.Add($"DTEND;TZID={TimeZoneId}:{FormatLocal(first, end)}");
        var interval = intervalWeeks > 1 ? $";INTERVAL={intervalWeeks}" : string.Empty;
        lines.Add($"RRULE:FREQ=WEEKLY{interval};BYDAY={ByDay(target)};UNTIL={FormatUtc(until)}");

        // Occurrences retirées (vacances, jours fériés...) : EXDATE ; aucune occurrence restante : pas d'événement.
        if (isActive is not null)
        {
            var occurrences = Library.Domain.SlotOccurrences.All(dayOfWeek, mondayFirst, firstDay, lastDay, intervalWeeks);
            var excluded = occurrences.Where(day => !isActive(day)).ToList();

            if (excluded.Count == occurrences.Count)
            {
                return null;
            }

            if (excluded.Count > 0)
            {
                lines.Add($"EXDATE;TZID={TimeZoneId}:{string.Join(',', excluded.Select(day => FormatLocal(day, start)))}");
            }
        }

        return Finish(lines, usesTimeZone: true);
    }

    /// <summary>Assemble un calendrier complet (abonnement) à partir d'événements déjà sérialisés.</summary>
    public static string Calendar(string name, IEnumerable<IcsEvent> events)
    {
        var list = events.ToList();
        var lines = new List<string>
        {
            "BEGIN:VCALENDAR",
            "VERSION:2.0",
            "PRODID:" + ProductId,
            "CALSCALE:GREGORIAN",
            "METHOD:PUBLISH",
            "X-WR-CALNAME:" + Escape(name),
            "X-WR-TIMEZONE:" + TimeZoneId,
            "REFRESH-INTERVAL;VALUE=DURATION:PT1H",
            "X-PUBLISHED-TTL:PT1H"
        };

        if (list.Any(e => e.UsesTimeZone))
        {
            lines.AddRange(VTimeZone);
        }

        var builder = new StringBuilder();
        foreach (var line in lines)
        {
            AppendFolded(builder, line);
        }

        foreach (var item in list)
        {
            builder.Append(item.Text);
        }

        AppendFolded(builder, "END:VCALENDAR");
        return builder.ToString();
    }

    private static void AddCommon(
        List<string> lines, string uid, DateTime changedUtc, string summary,
        string? description, string? location, string? category)
    {
        lines.Add("BEGIN:VEVENT");
        lines.Add("UID:" + uid);
        lines.Add("DTSTAMP:" + FormatUtc(DateTime.UtcNow));
        lines.Add("LAST-MODIFIED:" + FormatUtc(changedUtc));
        lines.Add("SUMMARY:" + Escape(summary));

        if (!string.IsNullOrWhiteSpace(description))
        {
            lines.Add("DESCRIPTION:" + Escape(description));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            lines.Add("LOCATION:" + Escape(location));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            lines.Add("CATEGORIES:" + Escape(category));
        }
    }

    private static IcsEvent Finish(List<string> lines, bool usesTimeZone)
    {
        lines.Add("END:VEVENT");

        var builder = new StringBuilder();
        foreach (var line in lines)
        {
            AppendFolded(builder, line);
        }

        return new IcsEvent(builder.ToString(), usesTimeZone);
    }

    /// <summary>Échappement d'une valeur TEXT (RFC 5545, 3.3.11).</summary>
    private static string Escape(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n")
            .Replace("\r", "\\n");
    }

    /// <summary>Découpe les lignes à 75 octets (UTF-8) sans couper un caractère (RFC 5545, 3.1).</summary>
    private static void AppendFolded(StringBuilder builder, string line)
    {
        const int limit = 75;
        var octets = 0;

        foreach (var rune in line.EnumerateRunes())
        {
            var size = rune.Utf8SequenceLength;
            if (octets + size > limit)
            {
                builder.Append("\r\n ");
                octets = 1;
            }

            builder.Append(rune.ToString());
            octets += size;
        }

        builder.Append("\r\n");
    }

    private static string FormatUtc(DateTime utc)
        => ParisTime.AsUtc(utc).ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);

    private static string FormatLocal(DateOnly day, TimeOnly time)
        => day.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
           + "T"
           + time.ToString("HHmmss", CultureInfo.InvariantCulture);

    /// <summary>Convertit le numéro de jour stocké (0 à 6) en jour de la semaine .NET.</summary>
    private static string ByDay(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "MO",
        DayOfWeek.Tuesday => "TU",
        DayOfWeek.Wednesday => "WE",
        DayOfWeek.Thursday => "TH",
        DayOfWeek.Friday => "FR",
        DayOfWeek.Saturday => "SA",
        _ => "SU"
    };
}
