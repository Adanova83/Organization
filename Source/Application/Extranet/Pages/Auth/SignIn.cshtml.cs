using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Library.Repository;

namespace Extranet.Components.Pages.Auth;

[ValidateAntiForgeryToken]
public class SignInModel : PageModel
{
    private readonly IMember _memberRepository;

    public SignInModel(IMember memberRepository)
    {
        _memberRepository = memberRepository;
    }

    // Bloque l'accès direct en GET
    //public IActionResult OnGet() => RedirectToPage("/login");

    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (!string.IsNullOrEmpty(error))
            ModelState.AddModelError(string.Empty, error);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        [FromForm] string email,
        [FromForm] string password,
        [FromForm] string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Redirect($"/login?error={Uri.EscapeDataString("Champs requis manquants.")}");

        try
        {
            var userResponse = await _memberRepository.ValidateUserAsync(email, password, default);
            if (userResponse?.Data == null)
                return Redirect($"/login?error={Uri.EscapeDataString("Email ou mot de passe incorrect.")}");

            var user = userResponse.Data;

            var rolesValidation = await _memberRepository.FetchRolesOfMemberAsync(user.Id, default);
            var permissionsValidation = await _memberRepository.FetchPermissionsOfMemberAsync(user.Id, default);

            if (rolesValidation?.Data == null || permissionsValidation?.Data == null)
                return Redirect($"/login?error={Uri.EscapeDataString("Erreur lors de la récupération des droits.")}");

            var claimsIdentity = new ClaimsIdentity(
                [
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, (user.Firstname + " " + user.Lastname).TrimStart()),
                    new(ClaimTypes.Email, user.Email)
                ],
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            claimsIdentity.AddClaims(rolesValidation.Data.Select(g => new Claim(ClaimTypes.GroupSid, g.Code)));
            claimsIdentity.AddClaims(permissionsValidation.Data.Select(r => new Claim(r.Type, r.Value)));

            var principal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            var safeReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
            return Redirect(safeReturnUrl);
        }
        catch (Exception ex)
        {
            return Redirect($"/login?error={Uri.EscapeDataString(ex.Message)}");
        }
    }
}