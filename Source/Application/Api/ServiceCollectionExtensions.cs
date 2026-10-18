using H = Library.Handler;
using R = Library.Repository;

namespace Api;

public static class ServiceCollectionExtensions
{
    /// <summary>Enregistre les repositories et handlers de la bibliothèque Library.</summary>
    public static IServiceCollection AddLibraryServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<R.IAccessEvent, R.AccessEvent>();
        services.AddScoped<R.IActivity, R.Activity>();
        services.AddScoped<R.IActivitySession, R.ActivitySession>();
        services.AddScoped<R.IActivitySessionSlot, R.ActivitySessionSlot>();
        services.AddScoped<R.IActivitySessionSlotAnimation, R.ActivitySessionSlotAnimation>();
        services.AddScoped<R.IActivitySessionSlotSubscription, R.ActivitySessionSlotSubscription>();
        services.AddScoped<R.IApplication, R.Application>();
        services.AddScoped<R.IAttendance, R.Attendance>();
        services.AddScoped<R.IBadge, R.Badge>();
        services.AddScoped<R.IBadgeAssignation, R.BadgeAssignation>();
        services.AddScoped<R.IContact, R.Contact>();
        services.AddScoped<R.IContribution, R.Contribution>();
        services.AddScoped<R.ICountry, R.Country>();
        services.AddScoped<R.IEvent, R.Event>();
        services.AddScoped<R.IEventAnimation, R.EventAnimation>();
        services.AddScoped<R.IEventSubscription, R.EventSubscription>();
        services.AddScoped<R.IHoliday, R.Holiday>();
        services.AddScoped<R.IMember, R.Member>();
        services.AddScoped<R.IMemberContact, R.MemberContact>();
        services.AddScoped<R.INote, R.Note>();
        services.AddScoped<R.INoteLink, R.NoteLink>();
        services.AddScoped<R.IOrganization, R.Organization>();
        services.AddScoped<R.IParticipant, R.Participant>();
        services.AddScoped<R.IOrganizationSession, R.OrganizationSession>();
        services.AddScoped<R.IProject, R.Project>();
        services.AddScoped<R.IProjectSlot, R.ProjectSlot>();
        services.AddScoped<R.IProjectSlotAnimation, R.ProjectSlotAnimation>();
        services.AddScoped<R.IProjectSlotSubscription, R.ProjectSlotSubscription>();
        services.AddScoped<R.IRole, R.Role>();
        services.AddScoped<R.ISchoolYear, R.SchoolYear>();
        services.AddScoped<R.ISpace, R.Space>();

        // Handlers
        services.AddScoped<H.AccessEvent>();
        services.AddScoped<H.Activity>();
        services.AddScoped<H.ActivitySession>();
        services.AddScoped<H.ActivitySessionSlot>();
        services.AddScoped<H.ActivitySessionSlotAnimation>();
        services.AddScoped<H.ActivitySessionSlotSubscription>();
        services.AddScoped<H.Application>();
        services.AddScoped<H.Attendance>();
        services.AddScoped<H.Badge>();
        services.AddScoped<H.BadgeAssignation>();
        services.AddScoped<H.Contact>();
        services.AddScoped<H.Contribution>();
        services.AddScoped<H.Country>();
        services.AddScoped<H.Event>();
        services.AddScoped<H.EventAnimation>();
        services.AddScoped<H.EventSubscription>();
        services.AddScoped<H.Holiday>();
        services.AddScoped<H.Member>();
        services.AddScoped<H.MemberContact>();
        services.AddScoped<H.Note>();
        services.AddScoped<H.NoteLink>();
        services.AddScoped<H.Organization>();
        services.AddScoped<H.OrganizationSession>();
        services.AddScoped<H.Participant>();
        services.AddScoped<H.Project>();
        services.AddScoped<H.ProjectSlot>();
        services.AddScoped<H.ProjectSlotAnimation>();
        services.AddScoped<H.ProjectSlotSubscription>();
        services.AddScoped<H.Role>();
        services.AddScoped<H.SchoolYear>();
        services.AddScoped<H.SubscriptionGuard>();
        services.AddScoped<H.Space>();

        return services;
    }
}
