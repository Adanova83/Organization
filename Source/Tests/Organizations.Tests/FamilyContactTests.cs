using Library.Data;
using Library.Entity;
using Microsoft.EntityFrameworkCore;
using Ct = Library.Contract;

namespace Organizations.Tests;

/// <summary>Contacts des familles : suppression en cascade, et participants vus par un animateur.</summary>
public class FamilyContactTests
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

    private static Member NewMember(Guid countryId, string lastname) => New(new Member
    {
        Lastname = lastname, Email = $"{lastname.ToLowerInvariant()}@example.org",
        Addr = new Address { Line1 = "1 rue", City = "Toulon", PostalCode = "83000", CountryId = countryId }
    });

    /// <summary>Deux enfants (frère et sœur) suivis par la même mère ; le premier est inscrit à un créneau animé par un troisième membre.</summary>
    private static async Task<(Member Child1, Member Child2, Member Animator, Contact Mother, ActivitySessionSlot Slot)> SeedAsync(FullDbContext db)
    {
        var country = New(new Country { CodeAlpha2 = "FR", CodeAlpha3 = "FRA", Name = "France" });
        var child1 = NewMember(country.Id, "Alice");
        var child2 = NewMember(country.Id, "Bruno");
        var animator = NewMember(country.Id, "Animateur");
        var mother = New(new Contact { Lastname = "Martin", Firstname = "Claire", PhoneNumber = "0600000000" });
        var organization = New(new Organization { Name = "Adanova" });
        var year = New(new SchoolYear { Code = "2026-2027", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(10) });
        var activity = New(new Activity { Code = "JEUX", Name = "Jeux", OrganizationId = organization.Id });
        var session = New(new ActivitySession { ActivityId = activity.Id, SchoolYearId = year.Id });
        var slot = New(new ActivitySessionSlot { SessionId = session.Id, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0) });

        db.AddRange(country, child1, child2, animator, mother, organization, year, activity, session, slot,
            New(new MemberContact { MemberId = child1.Id, ContactId = mother.Id, Relation = Library.Domain.ContactRelation.Mother, IsEmergencyContact = true }),
            New(new MemberContact { MemberId = child2.Id, ContactId = mother.Id, Relation = Library.Domain.ContactRelation.Mother, CanPickUp = true }),
            New(new ActivitySessionSlotSubscription { SessionSlotId = slot.Id, MemberId = child1.Id }),
            New(new ActivitySessionSlotAnimation { SessionSlotId = slot.Id, MemberId = animator.Id }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        return (child1, child2, animator, mother, slot);
    }

    [Fact]
    public async Task Deleting_a_member_removes_its_contact_links_but_keeps_the_shared_contact()
    {
        await using var db = NewContext();
        var seed = await SeedAsync(db);

        db.Members.Remove(await db.Members.SingleAsync(m => m.Id == seed.Child1.Id));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        Assert.Single(db.Contacts);
        Assert.Equal(seed.Child2.Id, Assert.Single(db.MemberContacts).MemberId);
    }

    [Fact]
    public async Task Deleting_a_contact_removes_it_from_every_member()
    {
        await using var db = NewContext();
        await SeedAsync(db);

        db.Contacts.Remove(await db.Contacts.SingleAsync());
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        Assert.Empty(db.MemberContacts);
    }

    [Fact]
    public async Task Participants_come_with_their_family_contacts_and_only_animators_are_recognised()
    {
        await using var db = NewContext();
        var seed = await SeedAsync(db);
        var repository = new Library.Repository.Participant(db);

        var participant = Assert.Single(await repository.ListAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, CancellationToken.None));
        Assert.Equal(seed.Child1.Id, participant.MemberId);
        var contact = Assert.Single(participant.Contacts);
        Assert.Equal("Martin", contact.Lastname);
        Assert.Equal(Ct.ContactRelation.Mother, contact.Relation);
        Assert.True(contact.IsEmergencyContact);

        Assert.True(await repository.IsAnimatorAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, seed.Animator.Id, CancellationToken.None));
        Assert.False(await repository.IsAnimatorAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, seed.Child2.Id, CancellationToken.None));
    }
}
