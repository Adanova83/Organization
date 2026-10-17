using Library.Data;
using Library.Entity;
using Microsoft.EntityFrameworkCore;
using InUseException = Library.Repository.InUseException;

namespace Organizations.Tests;

/// <summary>Politique de suppression (FullDbContextDeletion) : suppression logique propagée aux dépendants, ou refusée.</summary>
public class DeletionPolicyTests
{
    private static FullDbContext NewContext()
        => new(new DbContextOptionsBuilder<FullDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static T New<T>(T entity) where T : PrimaryKey
    {
        entity.Id = Guid.NewGuid();
        entity.Active = new Active();
        entity.Audit = new Audit();
        return entity;
    }

    /// <summary>Organisation, année, salle, pays, membre, activité avec une session, un créneau, une inscription et un animateur.</summary>
    private static async Task<(Activity Activity, ActivitySessionSlot Slot, Member Member, Space Space, Country Country)> SeedAsync(FullDbContext db)
    {
        var organization = New(new Organization { Name = "Adanova" });
        var year = New(new SchoolYear { Code = "2026-2027", StartDate = new DateTime(2026, 9, 1), EndDate = new DateTime(2027, 7, 4) });
        var space = New(new Space { Name = "Salle 1" });
        var country = New(new Country { CodeAlpha2 = "FR", CodeAlpha3 = "FRA", Name = "France" });
        var member = New(new Member
        {
            Lastname = "Test", Email = "test@example.org",
            Addr = new Address { Line1 = "1 rue", City = "Toulon", PostalCode = "83000", CountryId = country.Id }
        });
        var activity = New(new Activity { Code = "ROBOT", Name = "Robotique", OrganizationId = organization.Id });
        var session = New(new ActivitySession { ActivityId = activity.Id, SchoolYearId = year.Id });
        var slot = New(new ActivitySessionSlot { SessionId = session.Id, SpaceId = space.Id, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0) });

        db.AddRange(organization, year, space, country, member, activity, session, slot,
            New(new ActivitySessionSlotSubscription { SessionSlotId = slot.Id, MemberId = member.Id }),
            New(new ActivitySessionSlotAnimation { SessionSlotId = slot.Id, MemberId = member.Id }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        return (activity, slot, member, space, country);
    }

    [Fact]
    public async Task Deleting_an_activity_soft_deletes_its_sessions_slots_subscriptions_and_animations()
    {
        await using var db = NewContext();
        var seed = await SeedAsync(db);

        db.Activities.Remove(await db.Activities.SingleAsync(e => e.Id == seed.Activity.Id));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        Assert.Empty(db.Activities);
        Assert.Empty(db.ActivitySessions);
        Assert.Empty(db.ActivitySessionSlots);
        Assert.Empty(db.ActivitySessionSlotSubscriptions);
        Assert.Empty(db.ActivitySessionSlotAnimations);

        // Logique : les lignes restent, marquées supprimées.
        Assert.All(db.ActivitySessionSlotSubscriptions.IgnoreQueryFilters(), e => Assert.NotNull(e.Audit.DeletedAt));
        Assert.Single(db.ActivitySessions.IgnoreQueryFilters());
    }

    [Fact]
    public async Task Deleting_a_member_removes_participation_but_keeps_contributions()
    {
        await using var db = NewContext();
        var seed = await SeedAsync(db);
        var season = New(new OrganizationSession
        {
            OrganizationId = (await db.Activities.SingleAsync()).OrganizationId,
            SchoolYearId = (await db.SchoolYears.SingleAsync()).Id
        });
        db.AddRange(season, New(new Contribution { MemberId = seed.Member.Id, OrganizationSessionId = season.Id }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        db.Members.Remove(await db.Members.SingleAsync());
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        Assert.Empty(db.ActivitySessionSlotSubscriptions);
        Assert.Empty(db.ActivitySessionSlotAnimations);
        Assert.Single(db.Contributions);
        Assert.Single(db.ActivitySessionSlots);
    }

    [Fact]
    public async Task A_space_still_used_by_a_slot_cannot_be_deleted()
    {
        await using var db = NewContext();
        await SeedAsync(db);

        db.Spaces.Remove(await db.Spaces.SingleAsync());
        await Assert.ThrowsAsync<InUseException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task A_space_is_free_again_once_its_activity_is_deleted()
    {
        await using var db = NewContext();
        await SeedAsync(db);

        db.Activities.Remove(await db.Activities.SingleAsync());
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        db.Spaces.Remove(await db.Spaces.SingleAsync());
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        Assert.Empty(db.Spaces);
    }

    [Fact]
    public async Task A_country_used_in_a_member_address_cannot_be_deleted()
    {
        await using var db = NewContext();
        await SeedAsync(db);

        db.Countries.Remove(await db.Countries.SingleAsync());
        var refused = await Assert.ThrowsAsync<InUseException>(() => db.SaveChangesAsync());
        Assert.Contains("pays", refused.Message);
    }

    [Fact]
    public async Task A_school_year_or_an_organization_in_use_cannot_be_deleted()
    {
        await using var db = NewContext();
        await SeedAsync(db);

        db.SchoolYears.Remove(await db.SchoolYears.SingleAsync());
        await Assert.ThrowsAsync<InUseException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        db.Organizations.Remove(await db.Organizations.SingleAsync());
        await Assert.ThrowsAsync<InUseException>(() => db.SaveChangesAsync());
    }
}
