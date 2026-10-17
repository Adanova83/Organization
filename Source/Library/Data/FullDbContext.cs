using Library.Entity;
using Microsoft.EntityFrameworkCore;

namespace Library.Data;

public partial class FullDbContext : DbContext
{
    internal DbSet<AccessEvent> AccessEvents => Set<AccessEvent>();
    internal DbSet<Activity> Activities => Set<Activity>();
    internal DbSet<ActivitySession> ActivitySessions => Set<ActivitySession>();
    internal DbSet<ActivitySessionSlot> ActivitySessionSlots => Set<ActivitySessionSlot>();
    internal DbSet<ActivitySessionSlotAnimation> ActivitySessionSlotAnimations => Set<ActivitySessionSlotAnimation>();
    internal DbSet<ActivitySessionSlotSubscription> ActivitySessionSlotSubscriptions => Set<ActivitySessionSlotSubscription>();
    internal DbSet<Application> Applications => Set<Application>();
    internal DbSet<Badge> Badges => Set<Badge>();
    internal DbSet<BadgeAssignation> BadgeAssignations => Set<BadgeAssignation>();
    internal DbSet<Contribution> Contributions=> Set<Contribution>();
    internal DbSet<Country> Countries => Set<Country>();
    internal DbSet<Event> Events => Set<Event>();
    internal DbSet<EventAnimation> EventAnimations => Set<EventAnimation>();
    internal DbSet<EventSubscription> EventSubscriptions => Set<EventSubscription>();
    internal DbSet<Holiday> Holidays => Set<Holiday>();
    internal DbSet<Member> Members => Set<Member>();
    internal DbSet<Note> Notes => Set<Note>();
    internal DbSet<NoteLink> NoteLinks => Set<NoteLink>();
    internal DbSet<Organization> Organizations => Set<Organization>();
    internal DbSet<OrganizationSession> OrganizationSessions => Set<OrganizationSession>();
    internal DbSet<Project> Projects => Set<Project>();
    internal DbSet<ProjectSlot> ProjectSlots => Set<ProjectSlot>();
    internal DbSet<ProjectSlotAnimation> ProjectSlotAnimations => Set<ProjectSlotAnimation>();
    internal DbSet<ProjectSlotSubscription> ProjectSlotSubscriptions => Set<ProjectSlotSubscription>();
    internal DbSet<Role> Roles => Set<Role>();
    internal DbSet<SchoolYear> SchoolYears => Set<SchoolYear>();
    internal DbSet<Space> Spaces => Set<Space>();

    public FullDbContext(DbContextOptions<FullDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Organization");

        modelBuilder.Entity<AccessEvent>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Application>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Badge>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<BadgeAssignation>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Activity>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<ActivitySession>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<ActivitySessionSlot>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<ActivitySessionSlotAnimation>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<ActivitySessionSlotSubscription>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Contribution>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Country>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Event>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<EventAnimation>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<EventSubscription>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Holiday>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Member>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Note>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<NoteLink>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Organization>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<OrganizationSession>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Project>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<ProjectSlot>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<ProjectSlotAnimation>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<ProjectSlotSubscription>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<SchoolYear>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);
        modelBuilder.Entity<Space>().HasQueryFilter(e => !e.Audit.DeletedAt.HasValue);

        // Suppression logique : une ligne supprimée garde ses valeurs. Les index uniques ([Index(..., IsUnique = true)])
        // ne portent donc que sur les lignes non supprimées, pour pouvoir recréer un élément avec le même code, e-mail...
        foreach (var index in modelBuilder.Model.GetEntityTypes()
                     .Where(type => !type.IsOwned())
                     .SelectMany(type => type.GetIndexes())
                     .Where(index => index.IsUnique))
        {
            index.SetFilter("\"Audit_DeletedAt\" IS NULL");
        }
    }

    public override int SaveChanges()
    {
        PropagateDeletionsAsync(CancellationToken.None).GetAwaiter().GetResult();
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Politique de suppression (propagation ou refus, voir FullDbContextDeletion.cs), puis suppression logique.
        await PropagateDeletionsAsync(cancellationToken);
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var entries = ChangeTracker.Entries<PrimaryKey>()
            .Where(e => e.State == EntityState.Added
                     || e.State == EntityState.Modified
                     || e.State == EntityState.Deleted)
            .ToList();

        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            entity.Audit ??= new Audit();

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.Audit.CreatedAt = utcNow;
                    entity.Audit.LastUpdatedAt = utcNow;
                    break;
                case EntityState.Modified:
                    entity.Audit.LastUpdatedAt = utcNow;
                    // CreatedAt ne doit jamais changer après la création.
                    entry.Reference(e => e.Audit).TargetEntry!
                        .Property(nameof(Audit.CreatedAt)).IsModified = false;
                    break;
                case EntityState.Deleted:
                    // Suppression logique : on conserve la ligne et on renseigne DeletedAt.
                    entry.State = EntityState.Modified;

                    // Remove() a aussi marqué les types détenus (Audit, Active, Addr...) comme supprimés :
                    // on les remet à Unchanged, seul Audit est enregistré comme modifié.
                    foreach (var reference in entry.References)
                    {
                        if (reference.TargetEntry != null && reference.Metadata.TargetEntityType.IsOwned())
                        {
                            reference.TargetEntry.State = EntityState.Unchanged;
                        }
                    }

                    entity.Audit.LastUpdatedAt = utcNow;
                    entity.Audit.DeletedAt = utcNow;
                    entry.Reference(e => e.Audit).TargetEntry!.State = EntityState.Modified;
                    break;
            }
        }
    }
}
