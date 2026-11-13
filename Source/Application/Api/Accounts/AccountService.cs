using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Ct = Library.Contract;
using Do = Library.Domain;
using H = Library.Handler;

namespace Api.Accounts;

/// <summary>Demande refusée (compte déjà créé, compte administrateur protégé, liaison non configurée...).</summary>
public sealed class AccountConflictException(string message) : Exception(message);

/// <summary>
/// Comptes des membres dans l'annuaire (Authentik), pilotés depuis l'intranet :
/// création avec un mot de passe provisoire (ou rattachement d'un compte existant de même e-mail), groupe des animateurs,
/// désactivation, nouveau mot de passe provisoire. Nom et e-mail suivent la fiche ; supprimer le membre désactive le compte.
/// Les comptes administrateurs d'Authentik ne sont jamais modifiés par l'intranet.
/// </summary>
public sealed class AccountService
{
    private const string PasswordAlphabet = "abcdefghjkmnpqrstuvwxyz23456789";

    private readonly IDirectoryClient _directory;
    private readonly DirectoryOptions _options;
    private readonly H.Member _members;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IDirectoryClient directory, IOptions<DirectoryOptions> options, H.Member members, ILogger<AccountService> logger)
    {
        _directory = directory;
        _options = options.Value;
        _members = members;
        _logger = logger;
    }

    public bool Enabled => _options.IsConfigured;

    public async Task<Ct.MemberAccount> GetAsync(Do.Member member, CancellationToken ct)
    {
        if (!Enabled)
        {
            return new Ct.MemberAccount();
        }

        var user = await FindLinkedAsync(member, ct);
        return user is null ? new Ct.MemberAccount { Enabled = true } : await DescribeAsync(user, ct);
    }

    /// <summary>
    /// Crée le compte du membre (groupe des membres, et des animateurs si demandé) avec un mot de passe provisoire.
    /// Un compte de même e-mail existant déjà dans l'annuaire est rattaché tel quel, sans changer son mot de passe.
    /// </summary>
    public async Task<Ct.MemberAccount> CreateAsync(Do.Member member, bool isAnimator, CancellationToken ct)
    {
        EnsureEnabled();
        if (await FindLinkedAsync(member, ct) is not null)
        {
            throw new AccountConflictException("Ce membre a déjà un compte.");
        }

        if (string.IsNullOrWhiteSpace(member.Email))
        {
            throw new AccountConflictException("Le membre n'a pas d'adresse e-mail : impossible de lui créer un compte.");
        }

        string? password = null;
        var user = await _directory.FindUserByEmailAsync(member.Email, ct);
        if (user is null)
        {
            var username = await UniqueUsernameAsync(member, ct);
            user = await _directory.CreateUserAsync(username, DisplayName(member), member.Email, _options.UserPath, ct);
            password = NewPassword();
            await _directory.SetPasswordAsync(user.Id, password, ct);
        }

        await _members.SetDirectoryAccountAsync(member.Id, user.Id.ToString(CultureInfo.InvariantCulture), ct);

        if (!user.IsSuperuser)
        {
            await _directory.AddToGroupAsync(await _directory.GetGroupIdAsync(_options.MemberGroup, ct), user.Id, ct);
            if (isAnimator)
            {
                await _directory.AddToGroupAsync(await _directory.GetGroupIdAsync(_options.AnimatorGroup, ct), user.Id, ct);
            }

            if (!user.IsActive)
            {
                await _directory.SetActiveAsync(user.Id, true, ct);
            }

            user = await _directory.GetUserAsync(user.Id, ct) ?? user;
        }

        var account = await DescribeAsync(user, ct);
        account.TemporaryPassword = password;
        return account;
    }

    /// <summary>Groupe des animateurs et activation du compte.</summary>
    public async Task<Ct.MemberAccount> UpdateAsync(Do.Member member, Ct.MemberAccountUpdate update, CancellationToken ct)
    {
        var user = await RequireEditableAsync(member, ct);

        var animators = await _directory.GetGroupIdAsync(_options.AnimatorGroup, ct);
        var isAnimator = user.GroupIds.Contains(animators);
        if (update.IsAnimator && !isAnimator)
        {
            await _directory.AddToGroupAsync(animators, user.Id, ct);
        }
        else if (!update.IsAnimator && isAnimator)
        {
            await _directory.RemoveFromGroupAsync(animators, user.Id, ct);
        }

        if (update.IsActive != user.IsActive)
        {
            await _directory.SetActiveAsync(user.Id, update.IsActive, ct);
        }

        return await DescribeAsync(await _directory.GetUserAsync(user.Id, ct) ?? user, ct);
    }

    /// <summary>Nouveau mot de passe provisoire (oubli), renvoyé une seule fois.</summary>
    public async Task<Ct.MemberAccount> ResetPasswordAsync(Do.Member member, CancellationToken ct)
    {
        var user = await RequireEditableAsync(member, ct);
        var password = NewPassword();
        await _directory.SetPasswordAsync(user.Id, password, ct);

        var account = await DescribeAsync(user, ct);
        account.TemporaryPassword = password;
        return account;
    }

    /// <summary>
    /// Après une modification ou une suppression de la fiche : nom et e-mail recopiés dans le compte, ou compte désactivé
    /// si le membre est supprimé. Un échec est journalisé sans bloquer la modification de la fiche.
    /// </summary>
    public async Task SyncAsync(Do.Member member, CancellationToken ct)
    {
        if (!Enabled || string.IsNullOrEmpty(member.DirectoryAccountId))
        {
            return;
        }

        try
        {
            var user = await FindLinkedAsync(member, ct);
            if (user is null || user.IsSuperuser)
            {
                return;
            }

            var current = (await _members.SearchAsync(member.Id, ct)).Data;
            if (current is null)
            {
                if (user.IsActive)
                {
                    await _directory.SetActiveAsync(user.Id, false, ct);
                }

                return;
            }

            var name = DisplayName(current);
            if (name != user.Name || !string.Equals(current.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                await _directory.UpdateUserAsync(user.Id, name, current.Email, ct);
            }
        }
        catch (DirectoryException ex)
        {
            _logger.LogWarning(ex, "Member {MemberId}: directory account not synchronised.", member.Id);
        }
    }

    /// <summary>Identifiant proposé : prenom.nom, en minuscules, sans accents ni espaces.</summary>
    public static string BaseUsername(string? firstname, string lastname)
    {
        var parts = new[] { firstname, lastname }.Select(Slug).Where(part => part.Length > 0);
        var username = string.Join(".", parts);
        return username.Length == 0 ? "membre" : username[..Math.Min(username.Length, 140)];
    }

    /// <summary>Mot de passe provisoire lisible (sans 0/o, 1/l/i) : trois groupes de quatre caractères, environ 60 bits.</summary>
    public static string NewPassword()
    {
        var groups = Enumerable.Range(0, 3).Select(_ =>
            new string(Enumerable.Range(0, 4).Select(_ => PasswordAlphabet[RandomNumberGenerator.GetInt32(PasswordAlphabet.Length)]).ToArray()));
        return string.Join("-", groups);
    }

    private static string Slug(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "";
        }

        var builder = new StringBuilder();
        foreach (var c in text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (c is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                builder.Append(c);
            }
            else if (c is ' ' or '-' or '\'' or '’' && builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder.ToString().Trim('-');
    }

    private static string DisplayName(Do.Member member) => $"{member.Firstname} {member.Lastname}".Trim();

    private async Task<string> UniqueUsernameAsync(Do.Member member, CancellationToken ct)
    {
        var baseName = BaseUsername(member.Firstname, member.Lastname);
        var candidate = baseName;
        for (var suffix = 2; await _directory.UsernameExistsAsync(candidate, ct); suffix++)
        {
            candidate = $"{baseName}{suffix}";
        }

        return candidate;
    }

    private async Task<DirectoryUser?> FindLinkedAsync(Do.Member member, CancellationToken ct)
        => int.TryParse(member.DirectoryAccountId, NumberStyles.None, CultureInfo.InvariantCulture, out var id)
            ? await _directory.GetUserAsync(id, ct)
            : null;

    private async Task<DirectoryUser> RequireEditableAsync(Do.Member member, CancellationToken ct)
    {
        EnsureEnabled();
        var user = await FindLinkedAsync(member, ct) ?? throw new AccountConflictException("Ce membre n'a pas encore de compte.");
        if (user.IsSuperuser)
        {
            throw new AccountConflictException("Compte administrateur d'Authentik : il se gère dans Authentik, pas depuis l'intranet.");
        }

        return user;
    }

    private async Task<Ct.MemberAccount> DescribeAsync(DirectoryUser user, CancellationToken ct) => new()
    {
        Enabled = true,
        Exists = true,
        Username = user.Username,
        IsActive = user.IsActive,
        IsAnimator = user.GroupIds.Contains(await _directory.GetGroupIdAsync(_options.AnimatorGroup, ct)),
        IsProtected = user.IsSuperuser
    };

    private void EnsureEnabled()
    {
        if (!Enabled)
        {
            throw new AccountConflictException("L'intranet n'est pas relié à Authentik : les comptes se gèrent dans Authentik.");
        }
    }
}
