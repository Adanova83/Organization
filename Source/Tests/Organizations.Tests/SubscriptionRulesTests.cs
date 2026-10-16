using Library.Domain;

namespace Organizations.Tests;

/// <summary>Règles d'inscription : doublons, capacité, âge minimum, dérogation des responsables.</summary>
public class SubscriptionRulesTests
{
    private static readonly Guid Me = Guid.NewGuid();
    private static readonly DateOnly Rentree = new(2026, 9, 1);

    private static void Check(IReadOnlyCollection<Guid> others, int? capacity = null, int ageMinimum = 0,
        DateOnly? birthdate = null, bool canOverride = false)
        => SubscriptionRules.Check(Me, others, capacity, ageMinimum, birthdate, Rentree, canOverride);

    private static Guid[] Others(int count) => Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToArray();

    [Theory]
    [InlineData(2016, 9, 1, 2026, 9, 1, 10)]
    [InlineData(2016, 9, 2, 2026, 9, 1, 9)]
    [InlineData(2016, 2, 29, 2026, 2, 28, 9)]
    [InlineData(2016, 2, 29, 2026, 3, 1, 10)]
    public void Age_revolu(int by, int bm, int bd, int oy, int om, int od, int expected)
        => Assert.Equal(expected, SubscriptionRules.AgeOn(new DateOnly(by, bm, bd), new DateOnly(oy, om, od)));

    [Fact]
    public void Age_apprecie_au_debut_ou_aujourd_hui_si_deja_commence()
    {
        var today = new DateOnly(2026, 10, 16);
        Assert.Equal(new DateOnly(2026, 11, 1), SubscriptionRules.AgeReference(new DateOnly(2026, 11, 1), today));
        Assert.Equal(today, SubscriptionRules.AgeReference(new DateOnly(2026, 9, 1), today));
        Assert.Equal(today, SubscriptionRules.AgeReference(null, today));
    }

    [Fact]
    public void Inscription_acceptee() => Check(Others(3), capacity: 10, ageMinimum: 6, birthdate: new DateOnly(2018, 1, 1));

    [Fact]
    public void Doublon_refuse_meme_pour_un_responsable()
    {
        var others = new[] { Me };
        Assert.Throws<SubscriptionRefusedException>(() => Check(others));
        Assert.Throws<SubscriptionRefusedException>(() => Check(others, canOverride: true));
    }

    [Fact]
    public void Complet()
    {
        var error = Assert.Throws<SubscriptionRefusedException>(() => Check(Others(12), capacity: 12));
        Assert.Contains("12 place", error.Message);
    }

    [Fact]
    public void Derniere_place_disponible() => Check(Others(11), capacity: 12);

    [Fact]
    public void Trop_jeune()
    {
        // 5 ans à la rentrée (6 ans le 2 septembre) : refusé pour « 6 ans et + ».
        var error = Assert.Throws<SubscriptionRefusedException>(() => Check([], ageMinimum: 6, birthdate: new DateOnly(2020, 9, 2)));
        Assert.Contains("6 ans", error.Message);
    }

    [Fact]
    public void Age_atteint_le_jour_du_debut() => Check([], ageMinimum: 6, birthdate: new DateOnly(2020, 9, 1));

    [Fact]
    public void Date_de_naissance_inconnue_refusee()
    {
        var error = Assert.Throws<SubscriptionRefusedException>(() => Check([], ageMinimum: 6));
        Assert.Contains("date de naissance", error.Message);
    }

    [Fact]
    public void Sans_age_minimum_la_naissance_n_est_pas_demandee() => Check([], ageMinimum: 0);

    [Fact]
    public void Un_responsable_deroge_a_la_capacite_et_a_l_age()
        => Check(Others(12), capacity: 12, ageMinimum: 6, birthdate: new DateOnly(2022, 1, 1), canOverride: true);
}
