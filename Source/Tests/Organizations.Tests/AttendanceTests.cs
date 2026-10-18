using Library.Data;
using Library.Entity;
using Microsoft.EntityFrameworkCore;
using Ct = Library.Contract;

namespace Organizations.Tests;

/// <summary>Présences : dates des séances d'un créneau, feuille d'émargement, appel et statistiques.</summary>
public class AttendanceTests
{
    // Mercredi 2 septembre 2026 ... mercredi 30 septembre 2026 ; vacances du 16 au 22 septembre (fictives).
    private static readonly DateOnly Sep2 = new(2026, 9, 2);
    private static readonly DateOnly Sep9 = new(2026, 9, 9);
    private static readonly DateOnly Sep16 = new(2026, 9, 16);
    private static readonly DateOnly Sep23 = new(2026, 9, 23);

    private static FullDbContext NewContext()
        => new(new DbContextOptionsBuilder<FullDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static T New<T>(T entity) where T : PrimaryKey
    {
        entity.Id = Guid.NewGuid();
        entity.Active = new Active();
        entity.Audit = new Audit { CreatedAt = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc) };
        return entity;
    }

    private static DateTime Utc(DateOnly day) => DateTime.SpecifyKind(day.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

    /// <summary>Créneau du mercredi (3 = mercredi, semaine commençant le dimanche), hors vacances, deux inscrits.</summary>
    private static async Task<(ActivitySessionSlot Slot, Member Alice, Member Bruno)> SeedAsync(FullDbContext db)
    {
        var country = New(new Country { CodeAlpha2 = "FR", CodeAlpha3 = "FRA", Name = "France" });
        Member NewMember(string name) => New(new Member
        {
            Lastname = name, Email = $"{name.ToLowerInvariant()}@example.org",
            Addr = new Address { Line1 = "1 rue", City = "Toulon", PostalCode = "83000", CountryId = country.Id }
        });

        var alice = NewMember("Alice");
        var bruno = NewMember("Bruno");
        var organization = New(new Organization { Name = "Adanova" });
        var year = New(new SchoolYear { Code = "2026-2027", StartDate = Utc(new DateOnly(2026, 9, 1)), EndDate = Utc(new DateOnly(2026, 9, 30)) });
        var activity = New(new Activity { Code = "ROBOT", Name = "Robotique", OrganizationId = organization.Id });
        var session = New(new ActivitySession { ActivityId = activity.Id, SchoolYearId = year.Id, DuringTermTime = true, DuringSchoolHolidays = false });
        var slot = New(new ActivitySessionSlot { SessionId = session.Id, DayOfWeek = 3, Frequency = 1, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0) });
        var holidays = New(new Holiday { Name = "Vacances", SchoolYearId = year.Id, StartDate = Utc(new DateOnly(2026, 9, 16)), EndDate = Utc(new DateOnly(2026, 9, 22)) });

        db.AddRange(country, alice, bruno, organization, year, activity, session, slot, holidays,
            New(new ActivitySessionSlotSubscription { SessionSlotId = slot.Id, MemberId = alice.Id }),
            New(new ActivitySessionSlotSubscription { SessionSlotId = slot.Id, MemberId = bruno.Id }));
        await db.SaveChangesAsync();

        // La base date les créations à l'enregistrement : inscriptions antidatées (faites avant la rentrée).
        foreach (var subscription in db.ActivitySessionSlotSubscriptions)
        {
            subscription.Audit.CreatedAt = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        return (slot, alice, bruno);
    }

    [Fact]
    public async Task Slot_sessions_follow_the_weekly_rhythm_without_school_holidays()
    {
        await using var db = NewContext();
        var seed = await SeedAsync(db);
        var repository = new Library.Repository.Attendance(db);

        var dates = await repository.OccurrenceDatesAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, mondayFirst: false, CancellationToken.None);

        Assert.Equal([Sep2, Sep9, Sep23, new DateOnly(2026, 9, 30)], dates);
        Assert.DoesNotContain(Sep16, dates!);
    }

    [Fact]
    public async Task Roll_call_is_recorded_once_per_member_and_can_be_corrected()
    {
        await using var db = NewContext();
        var seed = await SeedAsync(db);
        var handler = new Library.Handler.Attendance(new Library.Repository.Attendance(db));

        await handler.RecordAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, Sep9,
            [new Ct.AttendanceEntry { MemberId = seed.Alice.Id, Status = Ct.AttendanceStatus.Present },
             new Ct.AttendanceEntry { MemberId = seed.Bruno.Id, Status = Ct.AttendanceStatus.Absent }],
            Ct.AttendanceMethod.RollCall, mondayFirst: false, CancellationToken.None);

        // Correction : Bruno est finalement présent.
        await handler.RecordAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, Sep9,
            [new Ct.AttendanceEntry { MemberId = seed.Bruno.Id, Status = Ct.AttendanceStatus.Present }],
            Ct.AttendanceMethod.StaffCorrection, mondayFirst: false, CancellationToken.None);
        db.ChangeTracker.Clear();

        var sheet = await handler.SheetAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, Sep9, mondayFirst: false, CancellationToken.None);

        Assert.Equal(2, sheet!.Rows.Count);
        Assert.Equal(2, sheet.Present);
        Assert.Equal(Ct.AttendanceMethod.StaffCorrection, sheet.Rows.Single(r => r.MemberId == seed.Bruno.Id).Method);
        Assert.Equal(2, await db.Attendances.CountAsync());
    }

    [Fact]
    public async Task No_roll_call_on_a_day_without_session_nor_for_someone_not_subscribed()
    {
        await using var db = NewContext();
        var seed = await SeedAsync(db);
        var handler = new Library.Handler.Attendance(new Library.Repository.Attendance(db));
        var present = new Ct.AttendanceEntry { MemberId = seed.Alice.Id, Status = Ct.AttendanceStatus.Present };

        // Pendant les vacances, et un jeudi.
        await Assert.ThrowsAsync<ArgumentException>(() => handler.RecordAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, Sep16,
            [present], Ct.AttendanceMethod.RollCall, false, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() => handler.RecordAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, Sep9.AddDays(1),
            [present], Ct.AttendanceMethod.RollCall, false, CancellationToken.None));

        // Un membre qui n'est pas inscrit.
        await Assert.ThrowsAsync<ArgumentException>(() => handler.RecordAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, Sep9,
            [new Ct.AttendanceEntry { MemberId = Guid.NewGuid(), Status = Ct.AttendanceStatus.Present }],
            Ct.AttendanceMethod.RollCall, false, CancellationToken.None));
    }

    [Fact]
    public async Task Statistics_count_sessions_and_rates()
    {
        await using var db = NewContext();
        var seed = await SeedAsync(db);
        var handler = new Library.Handler.Attendance(new Library.Repository.Attendance(db));

        foreach (var (day, bruno) in new[] { (Sep2, Ct.AttendanceStatus.Absent), (Sep9, Ct.AttendanceStatus.Present) })
        {
            await handler.RecordAsync(Ct.SubscriptionTargetKind.ActivitySlot, seed.Slot.Id, day,
                [new Ct.AttendanceEntry { MemberId = seed.Alice.Id, Status = Ct.AttendanceStatus.Present },
                 new Ct.AttendanceEntry { MemberId = seed.Bruno.Id, Status = bruno }],
                Ct.AttendanceMethod.RollCall, false, CancellationToken.None);
        }

        var stats = await handler.StatsAsync(new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 30), CancellationToken.None);

        var target = Assert.Single(stats.Targets);
        Assert.Equal(2, target.Sessions);
        Assert.Equal(0.75, target.Rate);
        Assert.Equal(1.0, stats.Members.Single(m => m.MemberId == seed.Alice.Id).Rate);
        Assert.Equal(0.5, stats.Members.Single(m => m.MemberId == seed.Bruno.Id).Rate);
    }
}
