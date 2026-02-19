namespace Library.Repository;

public interface IBadgeAssignation : IRepository<Domain.BadgeAssignation>
{
    Task<RepositoryResponseRange<Domain.BadgeAssignation>> SearchAsync(Guid? badgeId, Guid? memberId, int page, int pageSize, CancellationToken ct);
}
