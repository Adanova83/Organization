using Library.Domain;

namespace Organizations.Tests;

/// <summary>Jours fériés légaux et date de Pâques.</summary>
public class FrenchPublicHolidaysTests
{
    [Theory]
    [InlineData(1961, 4, 2)]
    [InlineData(2000, 4, 23)]
    [InlineData(2008, 3, 23)]
    [InlineData(2011, 4, 24)]
    [InlineData(2019, 4, 21)]
    [InlineData(2024, 3, 31)]
    [InlineData(2025, 4, 20)]
    [InlineData(2026, 4, 5)]
    [InlineData(2027, 3, 28)]
    [InlineData(2038, 4, 25)]
    public void Dimanche_de_Paques(int year, int month, int day)
        => Assert.Equal(new DateOnly(year, month, day), FrenchPublicHolidays.EasterSunday(year));

    [Fact]
    public void Onze_jours_feries_en_2027()
    {
        var days = FrenchPublicHolidays.For(2027);

        Assert.Equal(11, days.Count);
        Assert.Equal(new DateOnly(2027, 3, 29), days.Single(d => d.Name == "Lundi de Pâques").Date);
        Assert.Equal(new DateOnly(2027, 5, 6), days.Single(d => d.Name == "Ascension").Date);
        Assert.Equal(new DateOnly(2027, 5, 17), days.Single(d => d.Name == "Lundi de Pentecôte").Date);
        Assert.Equal(days.OrderBy(d => d.Date), days);
    }

    [Fact]
    public void Alsace_Moselle_ajoute_deux_jours()
    {
        var days = FrenchPublicHolidays.For(2027, alsaceMoselle: true);

        Assert.Equal(13, days.Count);
        Assert.Contains(new PublicHoliday("Vendredi saint", new DateOnly(2027, 3, 26)), days);
        Assert.Contains(new PublicHoliday("Saint-Étienne", new DateOnly(2027, 12, 26)), days);
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(2201)]
    public void Annee_hors_limites(int year)
        => Assert.Throws<ArgumentOutOfRangeException>(() => FrenchPublicHolidays.For(year));
}
