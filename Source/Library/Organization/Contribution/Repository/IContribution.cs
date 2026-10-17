namespace Library.Repository;

public interface IContribution : IRepository<Domain.Contribution>
{
    Task<RepositoryResponseRange<Domain.Contribution>> SearchAsync(Guid memberId, int page, int pageSize, CancellationToken ct);

    /// <summary>Cotisations d'une saison par membre (null si la saison n'existe pas).</summary>
    Task<Contract.SeasonContributions?> SeasonAsync(Guid organizationSessionId, CancellationToken ct);
}
