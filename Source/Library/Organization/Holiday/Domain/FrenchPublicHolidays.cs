namespace Library.Domain;

/// <summary>Jour férié calculé : nom et date (jour civil, sans heure).</summary>
public sealed record PublicHoliday(string Name, DateOnly Date);

/// <summary>
/// Calcul des jours fériés légaux en France (Code du travail, art. L3133-1) pour une année donnée.
/// Les fêtes mobiles dépendent de la date de Pâques (calendrier grégorien).
/// Les vacances scolaires ne se calculent pas : elles sont fixées par arrêté, par zone.
/// </summary>
public static class FrenchPublicHolidays
{
    public const int MinYear = 1900;
    public const int MaxYear = 2200;

    /// <summary>Jours fériés de l'année, triés par date. <paramref name="alsaceMoselle"/> : ajoute le Vendredi saint et la Saint-Étienne (Bas-Rhin, Haut-Rhin, Moselle).</summary>
    public static IReadOnlyList<PublicHoliday> For(int year, bool alsaceMoselle = false)
    {
        if (year is < MinYear or > MaxYear)
        {
            throw new ArgumentOutOfRangeException(nameof(year), $"Year must be between {MinYear} and {MaxYear}.");
        }

        var easter = EasterSunday(year);

        var days = new List<PublicHoliday>
        {
            new("Jour de l'an", new DateOnly(year, 1, 1)),
            new("Lundi de Pâques", easter.AddDays(1)),
            new("Fête du Travail", new DateOnly(year, 5, 1)),
            new("Victoire 1945", new DateOnly(year, 5, 8)),
            new("Ascension", easter.AddDays(39)),
            new("Lundi de Pentecôte", easter.AddDays(50)),
            new("Fête nationale", new DateOnly(year, 7, 14)),
            new("Assomption", new DateOnly(year, 8, 15)),
            new("Toussaint", new DateOnly(year, 11, 1)),
            new("Armistice 1918", new DateOnly(year, 11, 11)),
            new("Noël", new DateOnly(year, 12, 25))
        };

        if (alsaceMoselle)
        {
            days.Add(new("Vendredi saint", easter.AddDays(-2)));
            days.Add(new("Saint-Étienne", new DateOnly(year, 12, 26)));
        }

        return days.OrderBy(day => day.Date).ToList();
    }

    /// <summary>Dimanche de Pâques (algorithme de Meeus / Jones / Butcher, calendrier grégorien).</summary>
    public static DateOnly EasterSunday(int year)
    {
        var a = year % 19;
        var b = year / 100;
        var c = year % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var month = (h + l - 7 * m + 114) / 31;
        var day = (h + l - 7 * m + 114) % 31 + 1;
        return new DateOnly(year, month, day);
    }
}
