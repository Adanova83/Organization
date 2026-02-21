using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using D = Library.Domain;
namespace Application.Server.Authentication;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
    private readonly IServiceProvider _serviceProvider;

    public CustomAuthStateProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Si déjà authentifié en mémoire, retourner l'état actuel
        if (_currentUser.Identity?.IsAuthenticated == true)
            return new AuthenticationState(_currentUser);

        // Sinon, lire depuis le cookie HTTP
        using var scope = _serviceProvider.CreateScope();
        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.User.Identity?.IsAuthenticated == true)
        {
            _currentUser = httpContext.User;
            return new AuthenticationState(_currentUser);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        using var scope = _serviceProvider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var pendingAuth = scope.ServiceProvider.GetRequiredService<PendingAuthSession>();

        var memberResult = await authService.ValidateUserAsync(email, password);
        if (memberResult.Data == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var rolesResult = await authService.FetchRolesOfMemberAsync(memberResult.Data.Id);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, memberResult.Data.Id.ToString()),
            new(ClaimTypes.Name, memberResult.Data.Name),
            new(ClaimTypes.Email, memberResult.Data.Email)
        };
        claims.AddRange(rolesResult.Data?.Select(r => new Claim(r.Type, r.Value)) ?? []);

        // Après la construction des claims
        Console.WriteLine($"[AUTH] Claims count: {claims.Count}");
        foreach (var claim in claims)
        {
            Console.WriteLine($"[AUTH] Claim: {claim.Type} = {claim.Value}");
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        // Retourner le token pour que Login.razor gère la navigation
        return pendingAuth.Store(_currentUser);
    }

    public async Task LogoutAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        if (httpContextAccessor.HttpContext != null)
        {
            await httpContextAccessor.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );
        }
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
