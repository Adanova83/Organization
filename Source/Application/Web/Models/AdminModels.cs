using System.ComponentModel.DataAnnotations;
using Ct = Library.Contract;

namespace Web.Models;

// Champs des formulaires des référentiels (organisations, saisons, jours fériés, pays, espaces), des cotisations,
// des notes et des badges, avec leurs contrôles de saisie (l'API refait les siens).
// Les dates « jour » sont saisies en date de Paris et enregistrées à minuit de Paris (en UTC).

public sealed class OrganizationModel
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string? Name { get; set; }

    public static OrganizationModel From(Ct.Organization source) => new() { Name = source.Name };

    public Ct.Organization ApplyTo(Ct.Organization target)
    {
        target.Name = Name!.Trim();
        return target;
    }
}

/// <summary>Année scolaire (2026-2027...), commune à toutes les organisations.</summary>
public sealed class SchoolYearModel : IValidatableObject
{
    [Required(ErrorMessage = "Le code est obligatoire.")]
    [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Le code est de la forme 2026-2027.")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "La date de début est obligatoire.")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "La date de fin est obligatoire.")]
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "La zone est obligatoire.")]
    [RegularExpression("^[ABC]$", ErrorMessage = "La zone est A, B ou C.")]
    public string? Zone { get; set; } = "B";

    /// <summary>Proposition pour une nouvelle année : celle qui suit la plus récente (ou l'année en cours), du 1er septembre au 4 juillet.</summary>
    public static SchoolYearModel Next(IEnumerable<Ct.SchoolYear> existing)
    {
        var latest = existing.Select(y => Format.ParisLocal(y.StartDate).Year).DefaultIfEmpty(0).Max();
        var today = DateTime.Today;
        var start = latest > 0 ? latest + 1 : (today.Month >= 8 ? today.Year : today.Year - 1);
        return new SchoolYearModel
        {
            Code = $"{start}-{start + 1}",
            StartDate = new DateTime(start, 9, 1),
            EndDate = new DateTime(start + 1, 7, 4),
            Zone = existing.OrderByDescending(y => y.StartDate).Select(y => y.Zone).FirstOrDefault() ?? "B"
        };
    }

    public static SchoolYearModel From(Ct.SchoolYear source) => new()
    {
        Code = source.Code,
        StartDate = Format.ParisLocal(source.StartDate).Date,
        EndDate = Format.ParisLocal(source.EndDate).Date,
        Zone = source.Zone
    };

    public Ct.SchoolYear ApplyTo(Ct.SchoolYear target)
    {
        target.Zone = Zone!;
        target.Code = Code!.Trim();
        target.StartDate = Format.ParisToUtc(StartDate!.Value.Date);
        target.EndDate = Format.ParisToUtc(EndDate!.Value.Date);
        return target;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (StartDate is { } start && EndDate is { } end && end < start)
        {
            yield return new ValidationResult("La date de fin doit suivre la date de début.", [nameof(EndDate)]);
        }
    }
}

/// <summary>Saison d'une organisation (OrganizationSession) : son année scolaire et la cotisation.</summary>
public sealed class SeasonModel
{
    [Required(ErrorMessage = "L'année scolaire est obligatoire.")]
    public Guid? SchoolYearId { get; set; }

    [Range(0, 10000, ErrorMessage = "La cotisation doit être positive.")]
    public float Price { get; set; }

    public static SeasonModel From(Ct.OrganizationSession source) => new()
    {
        SchoolYearId = source.SchoolYearId == Guid.Empty ? null : source.SchoolYearId,
        Price = source.Price
    };

    public Ct.OrganizationSession ApplyTo(Ct.OrganizationSession target)
    {
        target.SchoolYearId = SchoolYearId!.Value;
        target.Price = Price;
        return target;
    }
}

public sealed class HolidayModel : IValidatableObject
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "La date de début est obligatoire.")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "La date de fin est obligatoire.")]
    public DateTime? EndDate { get; set; }

    /// <summary>Année scolaire, facultative (vacances scolaires) ; vide pour un jour férié.</summary>
    public Guid? SchoolYearId { get; set; }

    public static HolidayModel From(Ct.Holiday source) => new()
    {
        Name = source.Name,
        StartDate = Format.ParisLocal(source.StartDate).Date,
        EndDate = Format.ParisLocal(source.EndDate).Date,
        SchoolYearId = source.SchoolYearId
    };

    public Ct.Holiday ApplyTo(Ct.Holiday target)
    {
        target.Name = Name!.Trim();
        target.StartDate = Format.ParisToUtc(StartDate!.Value.Date);
        target.EndDate = Format.ParisToUtc(EndDate!.Value.Date);
        target.SchoolYearId = SchoolYearId;
        return target;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (StartDate is { } start && EndDate is { } end && end < start)
        {
            yield return new ValidationResult("La date de fin doit suivre la date de début.", [nameof(EndDate)]);
        }
    }
}

/// <summary>Génération automatique des jours fériés légaux d'une année.</summary>
public sealed class HolidayGenerationModel
{
    [Range(Library.Domain.FrenchPublicHolidays.MinYear, Library.Domain.FrenchPublicHolidays.MaxYear,
        ErrorMessage = "L'année doit être comprise entre 1900 et 2200.")]
    public int Year { get; set; } = DateTime.Today.Year;

    /// <summary>Ajoute le Vendredi saint et la Saint-Étienne (Bas-Rhin, Haut-Rhin, Moselle).</summary>
    public bool AlsaceMoselle { get; set; }
}

public sealed class CountryModel
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Le code à 2 lettres est obligatoire.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Le code doit faire 2 lettres.")]
    public string? CodeAlpha2 { get; set; }

    [Required(ErrorMessage = "Le code à 3 lettres est obligatoire.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Le code doit faire 3 lettres.")]
    public string? CodeAlpha3 { get; set; }

    public static CountryModel From(Ct.Country source) => new()
    {
        Name = source.Name,
        CodeAlpha2 = source.CodeAlpha2,
        CodeAlpha3 = source.CodeAlpha3
    };

    public Ct.Country ApplyTo(Ct.Country target)
    {
        target.Name = Name!.Trim();
        target.CodeAlpha2 = CodeAlpha2!.Trim().ToUpperInvariant();
        target.CodeAlpha3 = CodeAlpha3!.Trim().ToUpperInvariant();
        return target;
    }
}

public sealed class SpaceModel
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string? Name { get; set; }

    /// <summary>Couleur au format du champ de formulaire (#rrggbb).</summary>
    public string Color { get; set; } = "#ffffff";

    public static SpaceModel From(Ct.Space source) => new() { Name = source.Name, Color = Colors.ToInput(source.RGBColor) };

    public Ct.Space ApplyTo(Ct.Space target)
    {
        target.Name = Name!.Trim();
        target.RGBColor = Colors.FromInput(Color);
        return target;
    }
}

public sealed class ContributionModel
{
    [Required(ErrorMessage = "La saison est obligatoire.")]
    public Guid? OrganizationSessionId { get; set; }

    [Required(ErrorMessage = "La date de paiement est obligatoire.")]
    public DateOnly? PayedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    /// <summary>Vide : tarif de la saison.</summary>
    [Range(0, 100000, ErrorMessage = "Le montant doit être positif.")]
    public float? Amount { get; set; }
}

public sealed class NoteModel
{
    [Required(ErrorMessage = "Le titre est obligatoire.")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Le texte est obligatoire.")]
    public string? Body { get; set; }

    public static NoteModel From(Ct.Note source) => new() { Title = source.Title, Body = source.Body };

    public Ct.Note ApplyTo(Ct.Note target)
    {
        target.Title = Title!.Trim();
        target.Body = Body!.Trim();
        return target;
    }
}

public sealed class BadgeModel
{
    [Required(ErrorMessage = "L'identifiant est obligatoire.")]
    public string? Identifier { get; set; }

    public static BadgeModel From(Ct.Badge source) => new() { Identifier = source.Identifier };

    public Ct.Badge ApplyTo(Ct.Badge target)
    {
        target.Identifier = Identifier!.Trim();
        return target;
    }
}

/// <summary>Choix d'un membre (animateur, attribution de badge...).</summary>
public sealed class MemberChoiceModel
{
    [Required(ErrorMessage = "Choisissez un membre.")]
    public Guid? MemberId { get; set; }
}
