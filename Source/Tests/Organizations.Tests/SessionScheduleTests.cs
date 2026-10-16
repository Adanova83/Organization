using Library.Domain;

namespace Organizations.Tests;

/// <summary>Rythme des sessions et séries : hors vacances et/ou pendant les vacances scolaires, jamais les jours fériés.</summary>
public class SessionScheduleTests
{
    private static readonly ClosedPeriod Toussaint = new(new DateOnly(2026, 10, 17), new DateOnly(2026, 11, 1), IsSchoolHoliday: true);
    private static readonly ClosedPeriod Armistice = new(new DateOnly(2026, 11, 11), new DateOnly(2026, 11, 11), IsSchoolHoliday: false);
    private static readonly ClosedPeriod[] Periods = [Toussaint, Armistice];

    private static readonly DateOnly SchoolDay = new(2026, 11, 4);
    private static readonly DateOnly HolidayWeek = new(2026, 10, 24);
    private static readonly DateOnly PublicHoliday = new(2026, 11, 11);

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, true, false)]
    [InlineData(true, true, true)]
    public void Jour_de_classe(bool termTime, bool schoolHolidays, bool expected)
        => Assert.Equal(expected, SessionSchedule.IsActiveOn(SchoolDay, termTime, schoolHolidays, Periods));

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    public void Pendant_les_vacances_scolaires(bool termTime, bool schoolHolidays, bool expected)
        => Assert.Equal(expected, SessionSchedule.IsActiveOn(HolidayWeek, termTime, schoolHolidays, Periods));

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void Jamais_un_jour_ferie(bool termTime, bool schoolHolidays)
        => Assert.False(SessionSchedule.IsActiveOn(PublicHoliday, termTime, schoolHolidays, Periods));

    [Fact]
    public void Un_jour_ferie_pendant_les_vacances_reste_ferme()
    {
        var easterMonday = new ClosedPeriod(new DateOnly(2027, 3, 29), new DateOnly(2027, 3, 29), IsSchoolHoliday: false);
        var spring = new ClosedPeriod(new DateOnly(2027, 3, 27), new DateOnly(2027, 4, 11), IsSchoolHoliday: true);

        Assert.False(SessionSchedule.IsActiveOn(new DateOnly(2027, 3, 29), false, true, [spring, easterMonday]));
    }

    [Fact]
    public void Sans_periode_connue_seule_la_periode_scolaire_compte()
    {
        Assert.True(SessionSchedule.IsActiveOn(HolidayWeek, true, false, []));
        Assert.False(SessionSchedule.IsActiveOn(HolidayWeek, false, true, []));
    }

    [Theory]
    [InlineData(true, false, "Hors vacances scolaires")]
    [InlineData(false, true, "Vacances scolaires")]
    [InlineData(true, true, "Toute l'année")]
    public void Libelle(bool termTime, bool schoolHolidays, string expected)
        => Assert.Equal(expected, SessionSchedule.Label(termTime, schoolHolidays));
}
