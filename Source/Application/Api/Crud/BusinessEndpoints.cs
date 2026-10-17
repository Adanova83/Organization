using Api.Security;
using Ct = Library.Contract;
using Do = Library.Domain;
using H = Library.Handler;
using M = Library.Mapper;

namespace Api.Crud;

public static class BusinessEndpoints
{
    public static IEndpointRouteBuilder MapBusinessEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCrud<Ct.Space, Do.Space, H.Space>(
            "/api/spaces", "Spaces", Permissions.SpaceEdit,
            M.Space.ContractToDomain, M.Space.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchAsync(query, page, size, ct));

        var activities = app.MapCrud<Ct.Activity, Do.Activity, H.Activity>(
            "/api/activities", "Activities", Permissions.ActivityEdit,
            M.Activity.ContractToDomain, M.Activity.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchAsync(query, page, size, ct));

        activities.MapChildren<Ct.ActivitySession, Do.ActivitySession, H.ActivitySession>(
            "sessions",
            (handler, activityId, page, size, ct) => handler.SearchAsync(activityId, page, size, ct),
            M.ActivitySession.DomainToContract);

        var sessions = app.MapCrud<Ct.ActivitySession, Do.ActivitySession, H.ActivitySession>(
            "/api/activity-sessions", "ActivitySessions", Permissions.ActivityEdit,
            M.ActivitySession.ContractToDomain, M.ActivitySession.DomainToContract);

        sessions.MapChildren<Ct.ActivitySessionSlot, Do.ActivitySessionSlot, H.ActivitySessionSlot>(
            "slots",
            (handler, sessionId, page, size, ct) => handler.SearchAsync(sessionId, page, size, ct),
            M.ActivitySessionSlot.DomainToContract);

        app.MapCrud<Ct.ActivitySessionSlot, Do.ActivitySessionSlot, H.ActivitySessionSlot>(
            "/api/activity-session-slots", "ActivitySessionSlots", Permissions.ActivityEdit,
            M.ActivitySessionSlot.ContractToDomain, M.ActivitySessionSlot.DomainToContract,
            // Capacité augmentée : la liste d'attente prend les places libérées.
            afterChange: (slot, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().FillAsync(Ct.SubscriptionTargetKind.ActivitySlot, slot.Id, ct));

        // Membres : données personnelles (member.read pour lire, member.edit pour écrire).
        // ExternalId (lien avec le compte de l'annuaire) n'est jamais modifiable par ce biais.
        app.MapCrud<Ct.Member, Do.Member, H.Member>(
            "/api/members", "Members", Permissions.MemberEdit,
            M.Member.ContractToDomain, M.Member.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchAsync(query, page, size, ct),
            keepOnUpdate: (updated, existing) => updated.ExternalId = existing.ExternalId,
            // Membre supprimé avec ses inscriptions (voir FullDbContextDeletion) : ses places reviennent aux listes d'attente.
            afterChange: (_, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().FillAllAsync(ct));

        // ---- Référentiels et saisons ----
        // Années scolaires (2026-2027...) : communes à toutes les organisations ; saisons, sessions, événements, projets
        // et vacances scolaires s'y rattachent. Lecture : organization.read ; écriture : organization.edit.
        var schoolYears = app.MapCrud<Ct.SchoolYear, Do.SchoolYear, H.SchoolYear>(
            "/api/school-years", "SchoolYears", Permissions.OrganizationEdit,
            M.SchoolYear.ContractToDomain, M.SchoolYear.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchAsync(query, page, size, ct),
            afterCreate: SchoolHolidayImportEndpoints.ImportAfterCreateAsync);

        // Import des vacances scolaires officielles de l'année (POST /api/school-years/{id}/import-holidays).
        schoolYears.MapSchoolHolidayImport();

        schoolYears.MapChildren<Ct.OrganizationSession, Do.OrganizationSession, H.OrganizationSession>(
            "organization-sessions",
            (handler, schoolYearId, page, size, ct) => handler.SearchBySchoolYearAsync(schoolYearId, page, size, ct),
            M.OrganizationSession.DomainToContract);

        schoolYears.MapChildren<Ct.Holiday, Do.Holiday, H.Holiday>(
            "holidays",
            (handler, schoolYearId, page, size, ct) => handler.SearchBySchoolYearAsync(schoolYearId, page, size, ct),
            M.Holiday.DomainToContract);

        // Sessions, événements et projets d'une année : la lecture demande aussi la permission du domaine.
        schoolYears.MapChildren<Ct.ActivitySession, Do.ActivitySession, H.ActivitySession>(
                "activity-sessions",
                (handler, schoolYearId, page, size, ct) => handler.SearchBySchoolYearAsync(schoolYearId, page, size, ct),
                M.ActivitySession.DomainToContract,
                Permissions.ActivityRead);

        schoolYears.MapChildren<Ct.Event, Do.Event, H.Event>(
                "events",
                (handler, schoolYearId, page, size, ct) => handler.SearchBySchoolYearAsync(schoolYearId, page, size, ct),
                M.Event.DomainToContract,
                Permissions.EventRead);

        schoolYears.MapChildren<Ct.Project, Do.Project, H.Project>(
                "projects",
                (handler, schoolYearId, page, size, ct) => handler.SearchBySchoolYearAsync(schoolYearId, page, size, ct),
                M.Project.DomainToContract,
                Permissions.ProjectRead);

        app.MapCrud<Ct.Country, Do.Country, H.Country>(
            "/api/countries", "Countries", Permissions.OrganizationEdit,
            M.Country.ContractToDomain, M.Country.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchByNameAsync(query, page, size, ct));

        var organizations = app.MapCrud<Ct.Organization, Do.Organization, H.Organization>(
            "/api/organizations", "Organizations", Permissions.OrganizationEdit,
            M.Organization.ContractToDomain, M.Organization.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchAsync(query, page, size, ct));

        organizations.MapChildren<Ct.OrganizationSession, Do.OrganizationSession, H.OrganizationSession>(
            "sessions",
            (handler, organizationId, page, size, ct) => handler.SearchAsync(organizationId, page, size, ct),
            M.OrganizationSession.DomainToContract);

        app.MapCrud<Ct.OrganizationSession, Do.OrganizationSession, H.OrganizationSession>(
            "/api/organization-sessions", "OrganizationSessions", Permissions.OrganizationEdit,
            M.OrganizationSession.ContractToDomain, M.OrganizationSession.DomainToContract);

        var holidays = app.MapCrud<Ct.Holiday, Do.Holiday, H.Holiday>(
            "/api/holidays", "Holidays", Permissions.OrganizationEdit,
            M.Holiday.ContractToDomain, M.Holiday.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchAsync(query, page, size, ct));

        // Génération des jours fériés légaux d'une année (POST /api/holidays/generate).
        holidays.MapHolidayGeneration();

        // ---- Événements et projets ----
        var events = app.MapCrud<Ct.Event, Do.Event, H.Event>(
            "/api/events", "Events", Permissions.EventEdit,
            M.Event.ContractToDomain, M.Event.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchAsync(query, page, size, ct),
            afterChange: (item, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().FillAsync(Ct.SubscriptionTargetKind.Event, item.Id, ct));

        // Séries d'événements récurrents (POST /api/events/series...).
        events.MapEventSeries();

        var projects = app.MapCrud<Ct.Project, Do.Project, H.Project>(
            "/api/projects", "Projects", Permissions.ProjectEdit,
            M.Project.ContractToDomain, M.Project.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchAsync(query, page, size, ct));

        projects.MapChildren<Ct.ProjectSlot, Do.ProjectSlot, H.ProjectSlot>(
            "slots",
            (handler, projectId, page, size, ct) => handler.SearchAsync(projectId, page, size, ct),
            M.ProjectSlot.DomainToContract);

        app.MapCrud<Ct.ProjectSlot, Do.ProjectSlot, H.ProjectSlot>(
            "/api/project-slots", "ProjectSlots", Permissions.ProjectEdit,
            M.ProjectSlot.ContractToDomain, M.ProjectSlot.DomainToContract,
            afterChange: (slot, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().FillAsync(Ct.SubscriptionTargetKind.ProjectSlot, slot.Id, ct));

        // ---- Animateurs : lecture par *.read, écriture par la permission de l'activité concernée ----
        // Liste : ?targetId= (créneau ou événement) et/ou ?memberId= (animateur).
        app.MapCrud<Ct.ActivitySessionSlotAnimation, Do.ActivitySessionSlotAnimation, H.ActivitySessionSlotAnimation>(
                "/api/activity-session-slot-animations", "ActivitySessionSlotAnimations", Permissions.ActivityEdit,
                M.ActivitySessionSlotAnimation.ContractToDomain, M.ActivitySessionSlotAnimation.DomainToContract)
            .MapFilteredList<Ct.ActivitySessionSlotAnimation, Do.ActivitySessionSlotAnimation, H.ActivitySessionSlotAnimation>(
                (handler, targetId, memberId, page, size, ct) => handler.SearchAsync(targetId, memberId, page, size, ct),
                M.ActivitySessionSlotAnimation.DomainToContract);

        app.MapCrud<Ct.EventAnimation, Do.EventAnimation, H.EventAnimation>(
                "/api/event-animations", "EventAnimations", Permissions.EventEdit,
                M.EventAnimation.ContractToDomain, M.EventAnimation.DomainToContract)
            .MapFilteredList<Ct.EventAnimation, Do.EventAnimation, H.EventAnimation>(
                (handler, targetId, memberId, page, size, ct) => handler.SearchAsync(targetId, memberId, page, size, ct),
                M.EventAnimation.DomainToContract);

        app.MapCrud<Ct.ProjectSlotAnimation, Do.ProjectSlotAnimation, H.ProjectSlotAnimation>(
                "/api/project-slot-animations", "ProjectSlotAnimations", Permissions.ProjectEdit,
                M.ProjectSlotAnimation.ContractToDomain, M.ProjectSlotAnimation.DomainToContract)
            .MapFilteredList<Ct.ProjectSlotAnimation, Do.ProjectSlotAnimation, H.ProjectSlotAnimation>(
                (handler, targetId, memberId, page, size, ct) => handler.SearchAsync(targetId, memberId, page, size, ct),
                M.ProjectSlotAnimation.DomainToContract);

        // ---- Inscriptions : un membre gère les siennes ; subscription.edit permet de gérer celles de tous ----
        // Règles (SubscriptionGuard) : jamais deux fois le même membre ; capacité et âge minimum, sauf pour subscription.edit.
        app.MapSubscriptionAvailability();
        app.MapOwnedCrud<Ct.ActivitySessionSlotSubscription, Do.ActivitySessionSlotSubscription, H.ActivitySessionSlotSubscription>(
            "/api/activity-session-slot-subscriptions", "ActivitySessionSlotSubscriptions",
            Permissions.SubscriptionEdit, ownerCanWrite: true,
            M.ActivitySessionSlotSubscription.ContractToDomain, M.ActivitySessionSlotSubscription.DomainToContract,
            OwnedCrudEndpoints.OwnerField<Do.ActivitySessionSlotSubscription>(item => item.MemberId),
            (handler, targetId, memberId, page, size, ct) => handler.SearchAsync(targetId, memberId, page, size, ct),
            beforeWrite: (subscription, full, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().EnsureAsync(subscription, full, ct),
            afterDelete: (subscription, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().AfterDeleteAsync(subscription, ct));

        app.MapOwnedCrud<Ct.EventSubscription, Do.EventSubscription, H.EventSubscription>(
            "/api/event-subscriptions", "EventSubscriptions",
            Permissions.SubscriptionEdit, ownerCanWrite: true,
            M.EventSubscription.ContractToDomain, M.EventSubscription.DomainToContract,
            OwnedCrudEndpoints.OwnerField<Do.EventSubscription>(item => item.MemberId),
            (handler, targetId, memberId, page, size, ct) => handler.SearchAsync(targetId, memberId, page, size, ct),
            beforeWrite: (subscription, full, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().EnsureAsync(subscription, full, ct),
            afterDelete: (subscription, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().AfterDeleteAsync(subscription, ct));

        app.MapOwnedCrud<Ct.ProjectSlotSubscription, Do.ProjectSlotSubscription, H.ProjectSlotSubscription>(
            "/api/project-slot-subscriptions", "ProjectSlotSubscriptions",
            Permissions.SubscriptionEdit, ownerCanWrite: true,
            M.ProjectSlotSubscription.ContractToDomain, M.ProjectSlotSubscription.DomainToContract,
            OwnedCrudEndpoints.OwnerField<Do.ProjectSlotSubscription>(item => item.MemberId),
            (handler, targetId, memberId, page, size, ct) => handler.SearchAsync(targetId, memberId, page, size, ct),
            beforeWrite: (subscription, full, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().EnsureAsync(subscription, full, ct),
            afterDelete: (subscription, services, ct) => services.GetRequiredService<H.SubscriptionGuard>().AfterDeleteAsync(subscription, ct));

        // ---- Cotisations : un membre lit les siennes ; seul contribution.edit les écrit ----
        // La liste exige memberId (automatique pour un membre ordinaire).
        app.MapOwnedCrud<Ct.Contribution, Do.Contribution, H.Contribution>(
            "/api/contributions", "Contributions",
            Permissions.ContributionEdit, ownerCanWrite: false,
            M.Contribution.ContractToDomain, M.Contribution.DomainToContract,
            OwnedCrudEndpoints.OwnerField<Do.Contribution>(item => item.MemberId),
            (handler, targetId, memberId, page, size, ct) =>
                handler.SearchAsync(memberId ?? throw new ArgumentException("memberId is required."), page, size, ct));

        // ---- Notes personnelles : visibles et modifiables par leur seul auteur ----
        app.MapOwnedCrud<Ct.Note, Do.Note, H.Note>(
            "/api/notes", "Notes",
            staffPermission: null, ownerCanWrite: true,
            M.Note.ContractToDomain, M.Note.DomainToContract,
            OwnedCrudEndpoints.OwnerField<Do.Note>(item => item.MemberId),
            (handler, targetId, memberId, page, size, ct) =>
                handler.SearchAsync(memberId ?? throw new ArgumentException("memberId is required."), page, size, ct));

        // Liens d'une note : GET /api/note-links?targetId=<id de la note>. Le propriétaire est celui de la note.
        app.MapOwnedCrud<Ct.NoteLink, Do.NoteLink, H.NoteLink>(
            "/api/note-links", "NoteLinks",
            staffPermission: null, ownerCanWrite: true,
            M.NoteLink.ContractToDomain, M.NoteLink.DomainToContract,
            ownerOf: async (link, services, ct) => await NoteOwnerAsync(link.NoteId, services, ct),
            list: (handler, targetId, memberId, page, size, ct) =>
                handler.SearchAsync(targetId ?? throw new ArgumentException("targetId is required."), page, size, ct),
            ownerOfTarget: NoteOwnerAsync);

        // ---- Contrôle d'accès (badges) ----
        app.MapCrud<Ct.Badge, Do.Badge, H.Badge>(
            "/api/badges", "Badges", Permissions.AccessEdit,
            M.Badge.ContractToDomain, M.Badge.DomainToContract,
            (handler, query, page, size, ct) => handler.SearchAsync(query, page, size, ct));

        app.MapCrud<Ct.BadgeAssignation, Do.BadgeAssignation, H.BadgeAssignation>(
                "/api/badge-assignations", "BadgeAssignations", Permissions.AccessEdit,
                M.BadgeAssignation.ContractToDomain, M.BadgeAssignation.DomainToContract)
            .MapFilteredList<Ct.BadgeAssignation, Do.BadgeAssignation, H.BadgeAssignation>(
                (handler, targetId, memberId, page, size, ct) => handler.SearchAsync(targetId, memberId, page, size, ct),
                M.BadgeAssignation.DomainToContract);

        // Passages : lecture seule (ils sont enregistrés par les lecteurs, pas par les administrateurs).
        app.MapCrud<Ct.AccessEvent, Do.AccessEvent, H.AccessEvent>(
                "/api/access-events", "AccessEvents", Permissions.AccessEdit,
                M.AccessEvent.ContractToDomain, M.AccessEvent.DomainToContract,
                readOnly: true)
            .MapFilteredList<Ct.AccessEvent, Do.AccessEvent, H.AccessEvent>(
                (handler, targetId, memberId, page, size, ct) =>
                    handler.SearchAsync(targetId ?? throw new ArgumentException("targetId (badge) is required."), page, size, ct),
                M.AccessEvent.DomainToContract);

        return app;
    }

    private static async Task<Guid?> NoteOwnerAsync(Guid noteId, IServiceProvider services, CancellationToken ct)
    {
        var note = await services.GetRequiredService<H.Note>().SearchAsync(noteId, ct);
        return note.Data?.MemberId;
    }
}
