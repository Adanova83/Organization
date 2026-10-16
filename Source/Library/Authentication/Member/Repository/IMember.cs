namespace Library.Repository;

public  interface IMember : IRepository<Domain.Member>
{
    Task<RepositoryResponseRange<Domain.Member>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
    Task<RepositoryResponse<Domain.Member>> SearchByEmailAsync(string email, CancellationToken ct);
    Task<RepositoryResponse<Domain.Member>> SearchByExternalIdAsync(string externalId, CancellationToken ct);
    /// <summary>
    /// Rattache un compte de l'annuaire externe à un membre : retourne le membre déjà lié à <paramref name="externalId"/>,
    /// sinon le membre portant cet e-mail et non encore lié (il est alors lié), sinon aucun.
    /// </summary>
    Task<RepositoryResponse<Domain.Member>> LinkExternalAccountAsync(string externalId, string email, CancellationToken ct);
}
