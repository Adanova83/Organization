using D = Library.Domain;
using H = Library.Handler;
using R = Library.Repository;

namespace Application.Server.Authentication;

public class AuthenticationService : IAuthenticationService
{
    public R.IMember _member;

    public AuthenticationService(R.IMember member)
    {
        _member = member;
    }

    public async Task<R.RepositoryResponseRange<D.Role>> FetchRolesOfMemberAsync(Guid id, CancellationToken ct = default)
    {
        return await new H.Member(_member).FetchRolesOfMemberAsync(id, ct);
    }

    public async Task<R.RepositoryResponse<D.Member>> ValidateUserAsync(string email, string password, CancellationToken ct = default)
    {
        return await new H.Member(_member).ValidateUserAsync(email, password, ct);
    }
}
