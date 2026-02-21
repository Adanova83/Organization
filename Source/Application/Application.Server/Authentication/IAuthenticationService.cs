using System;
using D = Library.Domain;
using R = Library.Repository;

namespace Application.Server.Authentication;

public interface IAuthenticationService
{
    Task<R.RepositoryResponseRange<D.Role>> FetchRolesOfMemberAsync(Guid id, CancellationToken ct = default);
    Task<R.RepositoryResponse<D.Member>> ValidateUserAsync(string email, string password, CancellationToken ct = default);
}
