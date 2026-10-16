using System.ComponentModel.DataAnnotations;
using Ct = Library.Contract;

namespace Web.Models;

/// <summary>Champs du formulaire d'un membre, avec leurs contrôles de saisie. L'API refait ses propres contrôles.</summary>
public sealed class MemberModel
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string? Lastname { get; set; }

    public string? Firstname { get; set; }

    [Required(ErrorMessage = "L'e-mail est obligatoire.")]
    [EmailAddress(ErrorMessage = "L'e-mail n'est pas valide.")]
    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? Birthdate { get; set; }

    [Required(ErrorMessage = "L'adresse est obligatoire.")]
    public string? Line1 { get; set; }

    public string? Line2 { get; set; }

    public string? Line3 { get; set; }

    [Required(ErrorMessage = "La ville est obligatoire.")]
    public string? City { get; set; }

    [Required(ErrorMessage = "Le code postal est obligatoire.")]
    public string? PostalCode { get; set; }

    [Required(ErrorMessage = "Le pays est obligatoire.")]
    public Guid? CountryId { get; set; }

    public static MemberModel From(Ct.Member member) => new()
    {
        Lastname = member.Lastname,
        Firstname = member.Firstname,
        Email = member.Email,
        PhoneNumber = member.PhoneNumber,
        Birthdate = member.Birthdate,
        Line1 = member.Addr.Line1,
        Line2 = member.Addr.Line2,
        Line3 = member.Addr.Line3,
        City = member.Addr.City,
        PostalCode = member.Addr.PostalCode,
        CountryId = member.Addr.CountryId == Guid.Empty ? null : member.Addr.CountryId
    };

    /// <summary>Reporte la saisie sur un membre (existant : ses autres champs, image, couleur, période, sont conservés).</summary>
    public Ct.Member ApplyTo(Ct.Member member)
    {
        member.Lastname = Lastname!.Trim();
        member.Firstname = Clean(Firstname);
        member.Email = Email!.Trim();
        member.PhoneNumber = Clean(PhoneNumber);

        // La base stocke des dates UTC (timestamp with time zone) : une date sans fuseau serait refusée.
        member.Birthdate = Birthdate is { } date ? DateTime.SpecifyKind(date.Date, DateTimeKind.Utc) : null;

        member.Addr ??= new Ct.Address();
        member.Addr.Line1 = Line1!.Trim();
        member.Addr.Line2 = Clean(Line2);
        member.Addr.Line3 = Clean(Line3);
        member.Addr.City = City!.Trim();
        member.Addr.PostalCode = PostalCode!.Trim();
        member.Addr.CountryId = CountryId!.Value;

        return member;
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
