using Api.Calendar;

namespace Organizations.Tests;

/// <summary>Heure de Paris, dates des vacances importées et exclusions du flux ICS.</summary>
public class CalendarTests
{
    private static DateTime Utc(int year, int month, int day, int hour, int minute = 0)
        => new(year, month, day, hour, minute, 0, DateTimeKind.Utc);

    [Fact]
    public void Heure_de_Paris_en_hiver_et_en_ete()
    {
        Assert.Equal(Utc(2027, 1, 15, 9), ParisTime.ToUtc(new DateTime(2027, 1, 15, 10, 0, 0)));
        Assert.Equal(Utc(2027, 7, 15, 8), ParisTime.ToUtc(new DateTime(2027, 7, 15, 10, 0, 0)));
        Assert.Equal(new DateTime(2026, 10, 25, 10, 0, 0), ParisTime.ToLocal(Utc(2026, 10, 25, 9)));
    }

    [Fact]
    public void Vacances_du_premier_jour_a_la_veille_de_la_reprise()
    {
        // Toussaint 2026 (zone B) : du samedi 17 octobre, reprise le lundi 2 novembre.
        var (first, last) = SchoolHolidayImporter.Days(Utc(2026, 10, 16, 22), Utc(2026, 11, 1, 23), "Vacances de la Toussaint");

        Assert.Equal(new DateOnly(2026, 10, 17), first);
        Assert.Equal(new DateOnly(2026, 11, 1), last);
    }

    [Fact]
    public void Pont_d_un_jour()
    {
        var (first, last) = SchoolHolidayImporter.Days(Utc(2027, 5, 6, 22), Utc(2027, 5, 6, 22), "Pont de l'Ascension");

        Assert.Equal(new DateOnly(2027, 5, 7), first);
        Assert.Equal(first, last);
    }

    [Fact]
    public void Debut_des_vacances_d_ete_sans_fin_connue()
    {
        var (first, last) = SchoolHolidayImporter.Days(Utc(2028, 7, 3, 22), Utc(2028, 7, 3, 22), "Début des Vacances d'Été");

        Assert.Equal(new DateOnly(2028, 7, 4), first);
        Assert.Equal(new DateOnly(2028, 8, 31), last);
    }

    [Fact]
    public void Flux_ICS_exclut_les_dates_fermees()
    {
        // Samedis du 3 au 31 octobre 2026, 14 h - 16 h ; le 17 et le 24 sont en vacances.
        var closed = new HashSet<DateOnly> { new(2026, 10, 17), new(2026, 10, 24) };

        var ics = IcsBuilder.Weekly(
            "uid", Utc(2026, 10, 1, 0), "Atelier", null, null,
            dayOfWeek: 6, mondayFirst: false, Utc(2026, 10, 2, 22), Utc(2026, 10, 30, 23),
            new TimeOnly(14, 0), new TimeOnly(16, 0), 1, "Activité", day => !closed.Contains(day));

        Assert.NotNull(ics);
        var text = Unfold(ics.Text);
        Assert.Contains("EXDATE;TZID=Europe/Paris:20261017T140000,20261024T140000", text);
        Assert.Contains("DTSTART;TZID=Europe/Paris:20261003T140000", text);
    }

    [Fact]
    public void Flux_ICS_sans_date_restante()
    {
        var ics = IcsBuilder.Weekly(
            "uid", Utc(2026, 10, 1, 0), "Atelier", null, null,
            dayOfWeek: 6, mondayFirst: false, Utc(2026, 10, 2, 22), Utc(2026, 10, 30, 23),
            new TimeOnly(14, 0), new TimeOnly(16, 0), 1, "Activité", _ => false);

        Assert.Null(ics);
    }

    /// <summary>Lignes iCalendar dépliées (RFC 5545 : une ligne longue continue après « CRLF espace »).</summary>
    private static string Unfold(string text) => text.Replace("\r\n ", string.Empty);
}
