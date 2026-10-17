using Library.Contract;

namespace Organizations.Tests;

/// <summary>Cotisations d'une saison : état d'un membre (non payé, partiel, à jour) et totaux sur les participants.</summary>
public class SeasonContributionsTests
{
    private static MemberContribution Member(float due, float paid, bool participates = true)
        => new() { MemberId = Guid.NewGuid(), Lastname = "Test", Due = due, Paid = paid, Participates = participates };

    [Theory]
    [InlineData(120, 0, ContributionState.Unpaid, 120)]
    [InlineData(120, 50, ContributionState.Partial, 70)]
    [InlineData(120, 120, ContributionState.Paid, 0)]
    [InlineData(120, 150, ContributionState.Paid, 0)]
    [InlineData(0, 0, ContributionState.Paid, 0)]
    public void State_and_remaining_follow_what_was_paid(float due, float paid, ContributionState state, float remaining)
    {
        var member = Member(due, paid);

        Assert.Equal(state, member.State);
        Assert.Equal(remaining, member.Remaining);
    }

    [Fact]
    public void Totals_count_participants_but_collect_from_everyone()
    {
        var season = new SeasonContributions
        {
            Price = 100,
            Members = [Member(100, 100), Member(100, 40), Member(100, 0), Member(100, 100, participates: false)]
        };

        Assert.Equal(3, season.Participants);
        Assert.Equal(1, season.ParticipantsUpToDate);
        Assert.Equal(2, season.ParticipantsNotUpToDate);
        Assert.Equal(240, season.Collected);
        Assert.Equal(300, season.Expected);
    }
}
