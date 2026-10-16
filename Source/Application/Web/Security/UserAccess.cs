using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Web.Security;

/// <summary>Permet à une page de savoir si l'utilisateur a une permission (pour afficher ou masquer boutons et liens).</summary>
public sealed class UserAccess
{
    private readonly AuthenticationStateProvider _state;
    private readonly IAuthorizationService _authorization;

    public UserAccess(AuthenticationStateProvider state, IAuthorizationService authorization)
    {
        _state = state;
        _authorization = authorization;
    }

    public async Task<bool> CanAsync(string permission)
    {
        var user = (await _state.GetAuthenticationStateAsync()).User;
        return (await _authorization.AuthorizeAsync(user, permission)).Succeeded;
    }

    /// <summary>Âge du membre connecté, ou null si sa date de naissance est inconnue.</summary>
    public async Task<int?> AgeAsync()
    {
        var user = (await _state.GetAuthenticationStateAsync()).User;
        return DateTime.TryParse(user.FindFirst("birthdate")?.Value, null, System.Globalization.DateTimeStyles.RoundtripKind, out var birthdate)
            ? Models.Format.AgeYears(birthdate)
            : null;
    }

    /// <summary>Identifiant du membre connecté (claim member_id), ou null s'il n'a pas de membre.</summary>
    public async Task<Guid?> MemberIdAsync()
    {
        var user = (await _state.GetAuthenticationStateAsync()).User;
        return Guid.TryParse(user.FindFirst("member_id")?.Value, out var id) ? id : null;
    }
}
