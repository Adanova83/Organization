using System.Security.Claims;
using Microsoft.Extensions.Options;
using H = Library.Handler;
using Do = Library.Domain;

namespace Api.Security;

/// <summary>
/// Membre correspondant au compte connecté. Le compte est rattaché au membre à la première utilisation :
/// d'abord par identifiant de l'annuaire (<c>sub</c>), sinon par e-mail si celui-ci est vérifié.
/// Le résultat est conservé pour la durée de la requête.
/// </summary>
public sealed class CurrentMember
{
    private readonly H.Member _members;
    private readonly OidcOptions _options;
    private (Do.Member? Member, IResult? Error)? _cached;

    public CurrentMember(H.Member members, IOptions<OidcOptions> options)
    {
        _members = members;
        _options = options.Value;
    }

    /// <summary>Retourne le membre, ou la réponse d'erreur à renvoyer (401 sans sub, 403 sans membre lié).</summary>
    public async Task<(Do.Member? Member, IResult? Error)> FindAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        if (_cached.HasValue)
        {
            return _cached.Value;
        }

        var subject = user.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(subject))
        {
            return (null, Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "The token has no 'sub' claim."));
        }

        var email = user.FindFirst(_options.EmailClaim)?.Value;
        var emailVerified = user.FindFirst("email_verified")?.Value;
        var canLinkByEmail = !string.IsNullOrWhiteSpace(email)
            && !string.Equals(emailVerified, "false", StringComparison.OrdinalIgnoreCase);

        var response = canLinkByEmail
            ? await _members.LinkExternalAccountAsync(subject, email!, ct)
            : await _members.SearchByExternalIdAsync(subject, ct);

        _cached = response.Data is null
            ? (null, Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "No member is linked to this account.",
                detail: "Ask an administrator to create a member with the e-mail address of your account."))
            : (response.Data, null);

        return _cached.Value;
    }
}
