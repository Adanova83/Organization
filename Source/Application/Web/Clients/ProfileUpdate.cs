using Ct = Library.Contract;

namespace Web.Clients;

/// <summary>Corps de PUT /api/me : ce que le membre peut modifier lui-même (ni l'e-mail ni le lien avec l'annuaire).</summary>
public sealed class ProfileUpdate
{
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = default!;
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public DateTime? Birthdate { get; set; }
    public string? PhoneNumber { get; set; }
    public Ct.Address Addr { get; set; } = default!;
}
