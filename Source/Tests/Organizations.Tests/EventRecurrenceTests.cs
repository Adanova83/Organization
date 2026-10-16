using Library.Domain;

namespace Organizations.Tests;

/// <summary>Dates des séries d'événements récurrents (bornes incluses).</summary>
public class EventRecurrenceTests
{
    private static DateOnly D(int year, int month, int day) => new(year, month, day);

    [Fact]
    public void Dernier_vendredi_de_chaque_mois()
    {
        var dates = EventRecurrence.Dates(D(2026, 10, 16), D(2027, 7, 3), RecurrenceFrequency.MonthlyByWeekday, 1,
            DayOfWeek.Friday, weekOfMonth: EventRecurrence.LastWeekOfMonth);

        Assert.Equal(
            [D(2026, 10, 30), D(2026, 11, 27), D(2026, 12, 25), D(2027, 1, 29), D(2027, 2, 26),
             D(2027, 3, 26), D(2027, 4, 30), D(2027, 5, 28), D(2027, 6, 25)],
            dates);
    }

    [Fact]
    public void Premier_vendredi_de_chaque_mois()
    {
        var dates = EventRecurrence.Dates(D(2026, 11, 1), D(2027, 3, 31), RecurrenceFrequency.MonthlyByWeekday, 1,
            DayOfWeek.Friday, weekOfMonth: 1);

        Assert.Equal([D(2026, 11, 6), D(2026, 12, 4), D(2027, 1, 1), D(2027, 2, 5), D(2027, 3, 5)], dates);
    }

    [Fact]
    public void Deuxieme_mardi_un_mois_sur_deux()
    {
        var dates = EventRecurrence.Dates(D(2026, 9, 1), D(2027, 3, 31), RecurrenceFrequency.MonthlyByWeekday, 2,
            DayOfWeek.Tuesday, weekOfMonth: 2);

        Assert.Equal([D(2026, 9, 8), D(2026, 11, 10), D(2027, 1, 12), D(2027, 3, 9)], dates);
    }

    [Fact]
    public void Le_31_tombe_le_dernier_jour_des_mois_courts()
    {
        var dates = EventRecurrence.Dates(D(2026, 10, 16), D(2027, 6, 30), RecurrenceFrequency.MonthlyByDate, 1, dayOfMonth: 31);

        Assert.Equal(
            [D(2026, 10, 31), D(2026, 11, 30), D(2026, 12, 31), D(2027, 1, 31), D(2027, 2, 28),
             D(2027, 3, 31), D(2027, 4, 30), D(2027, 5, 31), D(2027, 6, 30)],
            dates);
    }

    [Fact]
    public void Une_date_du_mois_deja_passee_n_est_pas_retenue()
    {
        var dates = EventRecurrence.Dates(D(2026, 10, 16), D(2026, 12, 31), RecurrenceFrequency.MonthlyByDate, 1, dayOfMonth: 10);

        Assert.Equal([D(2026, 11, 10), D(2026, 12, 10)], dates);
    }

    [Fact]
    public void Mercredi_une_semaine_sur_deux()
    {
        var dates = EventRecurrence.Dates(D(2026, 10, 16), D(2026, 12, 1), RecurrenceFrequency.Weekly, 2, DayOfWeek.Wednesday);

        Assert.Equal([D(2026, 10, 21), D(2026, 11, 4), D(2026, 11, 18)], dates);
    }

    [Fact]
    public void Les_bornes_sont_incluses()
    {
        var dates = EventRecurrence.Dates(D(2026, 10, 17), D(2026, 10, 31), RecurrenceFrequency.Weekly, 1, DayOfWeek.Saturday);

        Assert.Equal([D(2026, 10, 17), D(2026, 10, 24), D(2026, 10, 31)], dates);
    }

    [Fact]
    public void Tous_les_trois_jours()
    {
        var dates = EventRecurrence.Dates(D(2026, 1, 1), D(2026, 1, 10), RecurrenceFrequency.Daily, 3);

        Assert.Equal([D(2026, 1, 1), D(2026, 1, 4), D(2026, 1, 7), D(2026, 1, 10)], dates);
    }

    [Fact]
    public void Chaque_semaine_demande_un_jour()
    {
        Assert.Throws<ArgumentException>(
            () => EventRecurrence.Dates(D(2026, 1, 1), D(2026, 2, 1), RecurrenceFrequency.Weekly, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(53)]
    public void Intervalle_hors_limites(int interval)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EventRecurrence.Dates(D(2026, 1, 1), D(2026, 2, 1), RecurrenceFrequency.Daily, interval));
    }

    [Fact]
    public void Fin_avant_debut()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EventRecurrence.Dates(D(2026, 2, 1), D(2026, 1, 1), RecurrenceFrequency.Daily, 1));
    }

    [Fact]
    public void Plus_de_deux_ans()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EventRecurrence.Dates(D(2026, 1, 1), D(2028, 1, 2), RecurrenceFrequency.Weekly, 1, DayOfWeek.Monday));
    }

    [Fact]
    public void Plus_de_200_dates()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EventRecurrence.Dates(D(2026, 1, 1), D(2026, 12, 31), RecurrenceFrequency.Daily, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void Jour_du_mois_hors_limites(int day)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EventRecurrence.Dates(D(2026, 1, 1), D(2026, 6, 1), RecurrenceFrequency.MonthlyByDate, 1, dayOfMonth: day));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Rang_dans_le_mois_hors_limites(int week)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EventRecurrence.Dates(D(2026, 1, 1), D(2026, 6, 1), RecurrenceFrequency.MonthlyByWeekday, 1, DayOfWeek.Friday, weekOfMonth: week));
    }
}
