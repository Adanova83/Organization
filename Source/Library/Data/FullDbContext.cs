using Library.Entity;
using Microsoft.EntityFrameworkCore;

namespace Library.Data;

public class FullDbContext : DbContext
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
    internal DbSet<Chat> Chats => Set<Chat>();
    internal DbSet<ChatMember> ChatMembers => Set<ChatMember>();
    internal DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    internal DbSet<ChatMessageRecipient> ChatMessageRecipients => Set<ChatMessageRecipient>();
    internal DbSet<Contribution> Contributions=> Set<Contribution>();
    internal DbSet<Country> Countries => Set<Country>();
    internal DbSet<Document> Documents => Set<Document>();
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
    internal DbSet<Space> Spaces => Set<Space>();

    public FullDbContext(DbContextOptions<FullDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Organization");

        modelBuilder.Entity<Activity>().HasQueryFilter(e => !e.Act.DeletedAt.HasValue);
        modelBuilder.Entity<ActivitySession>().HasQueryFilter(e => !e.Asi.DeletedAt.HasValue);
        modelBuilder.Entity<ActivitySessionSlot>().HasQueryFilter(e => !e.Asl.DeletedAt.HasValue);
        modelBuilder.Entity<ActivitySessionSlotAnimation>().HasQueryFilter(e => !e.Asa.DeletedAt.HasValue);
        modelBuilder.Entity<ActivitySessionSlotSubscription>().HasQueryFilter(e => !e.Ass.DeletedAt.HasValue);
        modelBuilder.Entity<Chat>().HasQueryFilter(e => !e.Cht.DeletedAt.HasValue);
        modelBuilder.Entity<ChatMember>().HasQueryFilter(e => !e.Cmm.DeletedAt.HasValue);
        modelBuilder.Entity<ChatMessage>().HasQueryFilter(e => !e.Chm.DeletedAt.HasValue);
        modelBuilder.Entity<ChatMessageRecipient>().HasQueryFilter(e => !e.Cmr.DeletedAt.HasValue);
        modelBuilder.Entity<Contribution>().HasQueryFilter(e => !e.Ctb.DeletedAt.HasValue);
        modelBuilder.Entity<Country>().HasQueryFilter(e => !e.Cnt.DeletedAt.HasValue);
        modelBuilder.Entity<Document>().HasQueryFilter(e => !e.Doc.DeletedAt.HasValue);
        modelBuilder.Entity<Event>().HasQueryFilter(e => !e.Evt.DeletedAt.HasValue);
        modelBuilder.Entity<EventAnimation>().HasQueryFilter(e => !e.Eva.DeletedAt.HasValue);
        modelBuilder.Entity<EventSubscription>().HasQueryFilter(e => !e.Evs.DeletedAt.HasValue);
        modelBuilder.Entity<Holiday>().HasQueryFilter(e => !e.Hld.DeletedAt.HasValue);
        modelBuilder.Entity<Member>().HasQueryFilter(e => !e.Mmb.DeletedAt.HasValue);
        modelBuilder.Entity<Note>().HasQueryFilter(e => !e.Nte.DeletedAt.HasValue);
        modelBuilder.Entity<NoteLink>().HasQueryFilter(e => !e.Ntl.DeletedAt.HasValue);
        modelBuilder.Entity<Organization>().HasQueryFilter(e => !e.Org.DeletedAt.HasValue);
        modelBuilder.Entity<OrganizationSession>().HasQueryFilter(e => !e.Ogs.DeletedAt.HasValue);
        modelBuilder.Entity<Project>().HasQueryFilter(e => !e.Prj.DeletedAt.HasValue);
        modelBuilder.Entity<ProjectSlot>().HasQueryFilter(e => !e.Prs.DeletedAt.HasValue);
        modelBuilder.Entity<ProjectSlotAnimation>().HasQueryFilter(e => !e.Psa.DeletedAt.HasValue);
        modelBuilder.Entity<ProjectSlotSubscription>().HasQueryFilter(e => !e.Pss.DeletedAt.HasValue);
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.Rol.DeletedAt.HasValue);
        modelBuilder.Entity<Space>().HasQueryFilter(e => !e.Spc.DeletedAt.HasValue);
    }

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Audit && (e.State == EntityState.Added || e.State == EntityState.Modified));

        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            var entity = (Audit)entry.Entity;

            switch(entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = utcNow;
                    entity.LastUpdatedAt = utcNow;
                    break;
                case EntityState.Modified:
                    entity.LastUpdatedAt = utcNow;
                    break;
                case EntityState.Deleted:
                    entity.DeletedAt = utcNow;
                    entry.State = EntityState.Modified;
                    break;
            }
        }
    }

    internal async Task SaveChangesAsync(object ct)
    {
        throw new NotImplementedException();
    }
}
