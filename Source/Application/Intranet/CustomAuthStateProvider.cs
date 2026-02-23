/*
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using R = Library.Repository;

namespace Intranet;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly R.IMember _member;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthStateProvider(R.IMember member, IHttpContextAccessor httpContextAccessor)
    {
        _member = member;
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (_httpContextAccessor.HttpContext!.Request.Cookies.ContainsKey(Constants.AuthCookieName))
            {
                var token = _httpContextAccessor.HttpContext.Request.Cookies[Constants.AuthCookieName];
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var claims = new List<Claim>();
                foreach (var claim in jsonToken!.Claims)
                {
                    claims.Add(new Claim(claim.Type, claim.Value));
                }

                var claimsIdentity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(claimsIdentity);
                return Task.FromResult(new AuthenticationState(user));
            }
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
        }
        catch (Exception)
        {
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
        }
    }

    public async Task<ClaimsIdentity> Login(string email, string password)
    {
        var userValidation = await _member.ValidateUserAsync(email, password, new CancellationToken());

        if (userValidation != null)
        {
            var user = userValidation.Data;
            if (user != null)
            {
                var groupsValidation = await _member.FetchGroupsOfMemberAsync(user.Id, new CancellationToken());
                var rolesValidation = await _member.FetchRolesOfMemberAsync(user.Id, new CancellationToken());
                if (groupsValidation != null && rolesValidation != null)
                {
                    var groups = groupsValidation.Data;
                    var roles = rolesValidation.Data;
                    var claimsIdentity = new ClaimsIdentity(
                        [
                            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new(ClaimTypes.Name, user.Name),
                            new(ClaimTypes.Email, user.Email)
                        ]
                    );

                    claimsIdentity.AddClaims(groups.Select(g => new Claim(ClaimTypes.GroupSid, g.Id.ToString())));
                    claimsIdentity.AddClaims(roles.Select(r => new Claim(r.Type, r.Value)));

                    return claimsIdentity;
                }
            }
        }

        throw new Exception("Invalid username or password");
    }
}
*/