using Library.Entity;
using InUseException = Library.Repository.InUseException;
using Microsoft.EntityFrameworkCore;

namespace Library.Data;

/// <summary>
/// Politique de suppression. Toute suppression est logique (Audit.DeletedAt, voir ApplyAuditInformation) ; avant
/// l'enregistrement, elle est propagée aux éléments qui n'ont pas de sens sans leur parent (sessions d'une activité,
/// inscriptions d'un créneau...), ou refusée pour un élément de référence encore utilisé (salle, pays, organisation...).
/// </summary>
public partial class FullDbContext
{
    /// <summary>Règle pour un type d'élément supprimé : éléments dépendants à supprimer aussi, ou refus s'il est encore utilisé.</summary>
    private sealed record DeletionRule(
        Type Parent,
        Func<FullDbContext, Guid, IQueryable<PrimaryKey>>? Dependents,
        Func<FullDbContext, Guid, CancellationToken, Task<bool>>? InUse,
        string Message);

    private static DeletionRule Cascade<TParent>(Func<FullDbContext, Guid, IQueryable<PrimaryKey>> dependents)
        => new(typeof(TParent), dependents, null, "");

    private static DeletionRule Block<TParent>(string message, Func<FullDbContext, Guid, CancellationToken, Task<bool>> inUse)
        => new(typeof(TParent), null, inUse, message);

    // Les refus sont listés avant les propagations : un élément refusé ne propage rien.
    private static readonly DeletionRule[] DeletionRules =
    [
        // ---- Éléments de référence : suppression refusée tant qu'ils sont utilisés ----
        Block<SchoolYear>("L'année scolaire est encore utilisée par des saisons, sessions, événements, projets ou vacances.",
            async (db, id, ct) => await db.OrganizationSessions.AnyAsync(e => e.SchoolYearId == id, ct)
                || await db.ActivitySessions.AnyAsync(e => e.SchoolYearId == id, ct)
                || await db.Events.AnyAsync(e => e.SchoolYearId == id, ct)
                || await db.Projects.AnyAsync(e => e.SchoolYearId == id, ct)
                || await db.Holidays.AnyAsync(e => e.SchoolYearId == id, ct)),
        Block<Organization>("L'organisation a encore des saisons, activités, événements ou projets : supprimez-les d'abord.",
            async (db, id, ct) => await db.OrganizationSessions.AnyAsync(e => e.OrganizationId == id, ct)
                || await db.Activities.AnyAsync(e => e.OrganizationId == id, ct)
                || await db.Events.AnyAsync(e => e.OrganizationId == id, ct)
                || await db.Projects.AnyAsync(e => e.OrganizationId == id, ct)),
        Block<OrganizationSession>("La saison a des cotisations enregistrées.",
            (db, id, ct) => db.Contributions.AnyAsync(e => e.OrganizationSessionId == id, ct)),
        Block<Space>("La salle est encore utilisée par des créneaux ou des événements.",
            async (db, id, ct) => await db.ActivitySessionSlots.AnyAsync(e => e.SpaceId == id, ct)
                || await db.Events.AnyAsync(e => e.SpaceId == id, ct)
                || await db.ProjectSlots.AnyAsync(e => e.SpaceId == id, ct)),
        Block<Country>("Le pays est encore utilisé dans l'adresse de membres.",
            (db, id, ct) => db.Members.AnyAsync(e => e.Addr.CountryId == id, ct)),

        // ---- Éléments composés : leurs dépendants sont supprimés avec eux ----
        Cascade<Activity>((db, id) => db.ActivitySessions.Where(e => e.ActivityId == id)),
        Cascade<ActivitySession>((db, id) => db.ActivitySessionSlots.Where(e => e.SessionId == id)),
        Cascade<ActivitySessionSlot>((db, id) => db.ActivitySessionSlotSubscriptions.Where(e => e.SessionSlotId == id)),
        Cascade<ActivitySessionSlot>((db, id) => db.ActivitySessionSlotAnimations.Where(e => e.SessionSlotId == id)),
        Cascade<Event>((db, id) => db.EventSubscriptions.Where(e => e.EventId == id)),
        Cascade<Project>((db, id) => db.ProjectSlots.Where(e => e.ProjectId == id)),
        Cascade<ProjectSlot>((db, id) => db.ProjectSlotSubscriptions.Where(e => e.ProjectSlotId == id)),
        Cascade<ProjectSlot>((db, id) => db.ProjectSlotAnimations.Where(e => e.ProjectSlotId == id)),
        Cascade<Note>((db, id) => db.NoteLinks.Where(e => e.NoteId == id)),
        // Badge : ses attributions ; l'historique des passages (AccessEvent) est conservé.
        Cascade<Badge>((db, id) => db.BadgeAssignations.Where(e => e.BadgeId == id)),
        // Contact d'une famille : ses rattachements aux membres.
        Cascade<Contact>((db, id) => db.MemberContacts.Where(e => e.ContactId == id)),

        // Membre : sa participation, ses données personnelles et ses contacts ; ses cotisations sont conservées (historique comptable).
        // Les présences (feuilles d'émargement) sont conservées dans tous les cas, comme les cotisations.
        Cascade<Member>((db, id) => db.ActivitySessionSlotSubscriptions.Where(e => e.MemberId == id)),
        Cascade<Member>((db, id) => db.EventSubscriptions.Where(e => e.MemberId == id)),
        Cascade<Member>((db, id) => db.ProjectSlotSubscriptions.Where(e => e.MemberId == id)),
        Cascade<Member>((db, id) => db.ActivitySessionSlotAnimations.Where(e => e.MemberId == id)),
        Cascade<Member>((db, id) => db.ProjectSlotAnimations.Where(e => e.MemberId == id)),
        Cascade<Member>((db, id) => db.Notes.Where(e => e.MemberId == id)),
        Cascade<Member>((db, id) => db.BadgeAssignations.Where(e => e.MemberId == id)),
        Cascade<Member>((db, id) => db.MemberContacts.Where(e => e.MemberId == id))
    ];

    /// <summary>
    /// Applique la politique aux éléments marqués supprimés, puis à leurs dépendants, de proche en proche.
    /// Les dépendants déjà supprimés (logiquement) sont exclus par le filtre global et ne sont pas repris.
    /// </summary>
    private async Task PropagateDeletionsAsync(CancellationToken ct)
    {
        var handled = new HashSet<PrimaryKey>(ReferenceEqualityComparer.Instance);

        while (true)
        {
            var deleted = ChangeTracker.Entries<PrimaryKey>()
                .Where(entry => entry.State == EntityState.Deleted && !handled.Contains(entry.Entity))
                .Select(entry => entry.Entity)
                .ToList();

            if (deleted.Count == 0)
            {
                return;
            }

            foreach (var entity in deleted)
            {
                handled.Add(entity);

                foreach (var rule in DeletionRules.Where(rule => rule.Parent.IsInstanceOfType(entity)))
                {
                    if (rule.InUse is not null && await rule.InUse(this, entity.Id, ct))
                    {
                        throw new InUseException(rule.Message);
                    }

                    if (rule.Dependents is not null)
                    {
                        foreach (var dependent in await rule.Dependents(this, entity.Id).ToListAsync(ct))
                        {
                            Remove(dependent);
                        }
                    }
                }
            }
        }
    }
}
