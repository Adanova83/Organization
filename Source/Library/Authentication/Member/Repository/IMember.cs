namespace Library.Repository;

public  interface IMember : IRepository<Domain.Member>
{
    Task<bool> AskNewPasswordAsync(string email, CancellationToken ct);
    Task<bool> ChangeEmailAsync(string email, string password, CancellationToken ct);
    Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword, CancellationToken ct);
    Task<bool> CreateNewPasswordAsync(Guid askedNewPassword, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.Role>> FetchRolesOfMemberAsync(Guid id, CancellationToken ct);
    Task<RepositoryResponseRange<Domain.Member>> SearchAsync(string? query, int page, int pageSize, CancellationToken ct);
    Task<RepositoryResponse<Domain.Member>> SearchByEmailAsync(string email, CancellationToken ct);
    Task<RepositoryResponse<Domain.Member>> ValidateUserAsync(string email, string password, CancellationToken ct);
}
