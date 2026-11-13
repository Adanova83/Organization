using Library.Data;
using Library.Entity;
using Microsoft.EntityFrameworkCore;
using Ct = Library.Contract;

namespace Organizations.Tests;

/// <summary>Agenda d'un membre : séances datées des créneaux qu'il anime ou suit, et événements auxquels il est inscrit.</summary>
public class AgendaTests
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

    private static DateTime Utc(int month, int day, int hour = 0) => new(2026, month, day, hour, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Agenda_lists_dated_sessions_with_roles_and_skips_school_holidays()
    {
        await using var db = NewContext();
        var country = New(new Country { CodeAlpha2 = "FR", CodeAlpha3 = "FRA", Name = "France" });
        var me = New(new Member { Lastname = "Moi", Email = "moi@example.org", Addr = new Address { Line1 = "1 rue", City = "Toulon", PostalCode = "83000", CountryId = country.Id } });
        var organization = New(new Organization { Name = "Adanova" });
        var year = New(new SchoolYear { Code = "2026-2027", StartDate = Utc(9, 1), EndDate = Utc(9, 30) });
        var activity = New(new Activity { Code = "ROBOT", Name = "Robotique", OrganizationId = organization.Id });
        var session = New(new ActivitySession { ActivityId = activity.Id, SchoolYearId = year.Id, DuringTermTime = true });
        // Mercredi (3 = mercredi, semaine commençant le dimanche) ; vacances du 16 au 22 septembre.
        var slot = New(new ActivitySessionSlot { SessionId = session.Id, DayOfWeek = 3, Frequency = 1, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0) });
        var holidays = New(new Holiday { Name = "Vacances", SchoolYearId = year.Id, StartDate = Utc(9, 16), EndDate = Utc(9, 22) });
        var party = New(new Event { Code = "FETE", Name = "Fête", OrganizationId = organization.Id, SchoolYearId = year.Id, StartDate = Utc(9, 12, 16), EndDate = Utc(9, 12, 20) });

        db.AddRange(country, me, organization, year, activity, session, slot, holidays, party,
            New(new ActivitySessionSlotAnimation { SessionSlotId = slot.Id, MemberId = me.Id }),
            New(new EventSubscription { EventId = party.Id, MemberId = me.Id, IsWaiting = true }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var agenda = await new Library.Repository.Agenda(db).ListAsync(me.Id, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 30), false, CancellationToken.None);

        var slots = agenda.Where(e => e.Kind == Ct.SubscriptionTargetKind.ActivitySlot).ToList();
        Assert.Equal([new DateOnly(2026, 9, 2), new DateOnly(2026, 9, 9), new DateOnly(2026, 9, 23), new DateOnly(2026, 9, 30)], slots.Select(e => e.Date));
        Assert.All(slots, e => Assert.Equal(Ct.AgendaRole.Animator, e.Role));
        Assert.All(slots, e => Assert.Equal(activity.Id, e.ParentId));

        var fete = Assert.Single(agenda, e => e.Kind == Ct.SubscriptionTargetKind.Event);
        Assert.Equal(new DateOnly(2026, 9, 12), fete.Date);
        Assert.True(fete.IsWaiting);
        Assert.Equal(new TimeOnly(18, 0), fete.StartTime); // 16 h UTC = 18 h à Paris (heure d'été)

        // Ordre chronologique : la fête du 12 entre les séances du 9 et du 23.
        Assert.Equal(fete, agenda[2]);
    }

    [Fact]
    public async Task Agenda_lists_one_off_project_slot_on_its_date_only()
    {
        await using var db = NewContext();
        var country = New(new Country { CodeAlpha2 = "FR", CodeAlpha3 = "FRA", Name = "France" });
        var me = New(new Member { Lastname = "Moi", Email = "moi@example.org", Addr = new Address { Line1 = "1 rue", City = "Toulon", PostalCode = "83000", CountryId = country.Id } });
        var organization = New(new Organization { Name = "Adanova" });
        var year = New(new SchoolYear { Code = "2026-2027", StartDate = Utc(9, 1), EndDate = Utc(9, 30) });
        var project = New(new Project { Code = "EXPO", Name = "Exposition", OrganizationId = organization.Id, SchoolYearId = year.Id, DuringTermTime = true });
        // Créneau ponctuel le samedi 19 septembre, pendant les vacances : retenu quand même (date choisie explicitement).
        var slot = New(new ProjectSlot { ProjectId = project.Id, DayOfWeek = 6, Date = new DateOnly(2026, 9, 19), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0) });
        var holidays = New(new Holiday { Name = "Vacances", SchoolYearId = year.Id, StartDate = Utc(9, 16), EndDate = Utc(9, 22) });

        db.AddRange(country, me, organization, year, project, slot, holidays,
            New(new ProjectSlotSubscription { ProjectSlotId = slot.Id, MemberId = me.Id }));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var agenda = await new Library.Repository.Agenda(db).ListAsync(me.Id, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 30), false, CancellationToken.None);

        var entry = Assert.Single(agenda);
        Assert.Equal(new DateOnly(2026, 9, 19), entry.Date);
        Assert.Equal(Ct.SubscriptionTargetKind.ProjectSlot, entry.Kind);
        Assert.Equal(project.Id, entry.ParentId);
    }
}
