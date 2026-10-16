using System.ComponentModel.DataAnnotations;
using Ct = Library.Contract;

namespace Web.Models;

// Champs des formulaires du planning, avec leurs contrôles de saisie (l'API refait les siens).
// Les dates « jour » sont saisies en date de Paris et enregistrées à minuit de Paris (en UTC) ;
// les dates avec heure sont saisies en heure de Paris.

public sealed class ActivityModel
{
    [Required(ErrorMessage = "Le code est obligatoire.")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    /// <summary>Couleur au format du champ de formulaire (#rrggbb).</summary>
    public string Color { get; set; } = "#ffffff";

    [Range(0, 120, ErrorMessage = "L'âge minimum doit être compris entre 0 et 120.")]
    public int AgeMinimum { get; set; }

    [Required(ErrorMessage = "L'organisation est obligatoire.")]
    public Guid? OrganizationId { get; set; }

    public static ActivityModel From(Ct.Activity source) => new()
    {
        Color = Colors.ToInput(source.RGBColor),
        Code = source.Code,
        Name = source.Name,
        Description = source.Description,
        AgeMinimum = source.AgeMinimum,
        OrganizationId = source.OrganizationId == Guid.Empty ? null : source.OrganizationId
    };

    public Ct.Activity ApplyTo(Ct.Activity target)
    {
        target.RGBColor = Colors.FromInput(Color);
        target.Code = Code!.Trim();
        target.Name = Name!.Trim();
        target.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
        target.AgeMinimum = AgeMinimum;
        target.OrganizationId = OrganizationId!.Value;
        return target;
    }
}

/// <summary>Session d'une activité pour une année scolaire. Dates facultatives : vides, la session couvre toute l'année.</summary>
public sealed class SessionModel : IValidatableObject
{
    [Required(ErrorMessage = "L'année scolaire est obligatoire.")]
    public Guid? SchoolYearId { get; set; }

    [Range(0, 10000, ErrorMessage = "Le tarif doit être positif.")]
    public float Price { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    /// <summary>En période scolaire (hors vacances). Pas de valeur par défaut ici : une case décochée n'est pas envoyée par le formulaire.</summary>
    public bool DuringTermTime { get; set; }

    /// <summary>Pendant les vacances scolaires.</summary>
    public bool DuringSchoolHolidays { get; set; }

    public static SessionModel From(Ct.ActivitySession source) => new()
    {
        SchoolYearId = source.SchoolYearId == Guid.Empty ? null : source.SchoolYearId,
        Price = source.Price,
        StartDate = Format.ParisDay(source.StartDate),
        EndDate = Format.ParisDay(source.EndDate),
        DuringTermTime = source.DuringTermTime,
        DuringSchoolHolidays = source.DuringSchoolHolidays
    };

    public Ct.ActivitySession ApplyTo(Ct.ActivitySession target)
    {
        target.SchoolYearId = SchoolYearId!.Value;
        target.Price = Price;
        target.StartDate = Format.DayToUtc(StartDate);
        target.EndDate = Format.DayToUtc(EndDate);
        target.DuringTermTime = DuringTermTime;
        target.DuringSchoolHolidays = DuringSchoolHolidays;
        return target;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (!DuringTermTime && !DuringSchoolHolidays)
        {
            yield return new ValidationResult("Cochez au moins « hors vacances scolaires » ou « pendant les vacances scolaires ».", [nameof(DuringTermTime)]);
        }

        if (StartDate is { } start && EndDate is { } end && end < start)
        {
            yield return new ValidationResult("La date de fin doit suivre la date de début.", [nameof(EndDate)]);
        }
    }
}

/// <summary>Créneau hebdomadaire (activité ou projet). Les heures sont saisies au format HH:mm.</summary>
public sealed class SlotModel : IValidatableObject
{
    [Range(0, 6, ErrorMessage = "Le jour est obligatoire.")]
    public int DayOfWeek { get; set; } = 1;

    [Required(ErrorMessage = "L'heure de début est obligatoire.")]
    public string? StartTime { get; set; }

    [Required(ErrorMessage = "L'heure de fin est obligatoire.")]
    public string? EndTime { get; set; }

    [Range(1, 52, ErrorMessage = "La fréquence est comprise entre 1 et 52 semaines.")]
    public int Frequency { get; set; } = 1;

    public Guid? SpaceId { get; set; }

    public static SlotModel From(Ct.ActivitySessionSlot source) => new()
    {
        DayOfWeek = source.DayOfWeek,
        StartTime = source.StartTime.ToString("HH:mm"),
        EndTime = source.EndTime.ToString("HH:mm"),
        Frequency = source.Frequency,
        SpaceId = source.SpaceId
    };

    public static SlotModel From(Ct.ProjectSlot source) => new()
    {
        DayOfWeek = source.DayOfWeek,
        StartTime = source.StartTime.ToString("HH:mm"),
        EndTime = source.EndTime.ToString("HH:mm"),
        SpaceId = source.SpaceId
    };

    public Ct.ActivitySessionSlot ApplyTo(Ct.ActivitySessionSlot target)
    {
        target.DayOfWeek = DayOfWeek;
        target.StartTime = Start;
        target.EndTime = End;
        target.Frequency = Frequency;
        target.SpaceId = SpaceId;
        return target;
    }

    public Ct.ProjectSlot ApplyTo(Ct.ProjectSlot target)
    {
        target.DayOfWeek = DayOfWeek;
        target.StartTime = Start;
        target.EndTime = End;
        target.SpaceId = SpaceId;
        return target;
    }

    private TimeOnly Start => TimeOnly.Parse(StartTime!);
    private TimeOnly End => TimeOnly.Parse(EndTime!);

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        var startOk = TimeOnly.TryParse(StartTime, out var start);
        var endOk = TimeOnly.TryParse(EndTime, out var end);

        if (!string.IsNullOrEmpty(StartTime) && !startOk)
        {
            yield return new ValidationResult("L'heure de début n'est pas valide.", [nameof(StartTime)]);
        }

        if (!string.IsNullOrEmpty(EndTime) && !endOk)
        {
            yield return new ValidationResult("L'heure de fin n'est pas valide.", [nameof(EndTime)]);
        }

        if (startOk && endOk && end <= start)
        {
            yield return new ValidationResult("L'heure de fin doit suivre l'heure de début.", [nameof(EndTime)]);
        }
    }
}

/// <summary>
/// Formulaire d'un événement. Événement unique : début et fin. Série (création seulement) : la période est celle de
/// l'année scolaire ; on saisit les heures et, selon la fréquence, le jour de la semaine, le numéro du jour ou son rang dans le mois.
/// </summary>
public sealed class EventModel : IValidatableObject
{
    [Required(ErrorMessage = "Le code est obligatoire.")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    // Événement unique : obligatoires (contrôle dans Validate).
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Range(0, 120, ErrorMessage = "L'âge minimum doit être compris entre 0 et 120.")]
    public int? AgeMinimum { get; set; }

    [Required(ErrorMessage = "L'organisation est obligatoire.")]
    public Guid? OrganizationId { get; set; }

    [Required(ErrorMessage = "L'année scolaire est obligatoire.")]
    public Guid? SchoolYearId { get; set; }

    public Guid? SpaceId { get; set; }

    /// <summary>Couleur au format du champ de formulaire (#rrggbb), pour le calendrier.</summary>
    public string Color { get; set; } = "#ffffff";

    // ---- Répétition (création seulement) : vide = événement unique ----

    public Ct.RecurrenceFrequency? Repeat { get; set; }

    /// <summary>Tous les N jours, semaines ou mois.</summary>
    [Range(1, 52, ErrorMessage = "Le nombre doit être compris entre 1 et 52.")]
    public int Interval { get; set; } = 1;

    /// <summary>Heures de chaque occurrence (HH:mm, heure de Paris).</summary>
    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    /// <summary>Chaque semaine : jour de la semaine (0 = dimanche … 6 = samedi).</summary>
    public int? WeeklyDay { get; set; }

    /// <summary>Chaque mois, même date : numéro du jour (1 à 31).</summary>
    public int? MonthDay { get; set; }

    /// <summary>Chaque mois, même jour : rang dans le mois (1 à 4, -1 = dernier) et jour de la semaine.</summary>
    public int? MonthRank { get; set; }

    public int? MonthWeekday { get; set; }

    /// <summary>Rythme : hors vacances scolaires et/ou pendant les vacances (jamais les jours fériés). Pas de valeur par défaut ici :
    /// une case décochée n'est pas envoyée par le formulaire ; « hors vacances » est cochée à la création du modèle.</summary>
    public bool DuringTermTime { get; set; }

    public bool DuringSchoolHolidays { get; set; }

    public static EventModel From(Ct.Event source) => new()
    {
        Color = Colors.ToInput(source.RGBColor),
        Code = source.Code,
        Name = source.Name,
        Description = source.Description,
        StartDate = Format.ParisLocal(source.StartDate),
        EndDate = Format.ParisLocal(source.EndDate),
        AgeMinimum = source.AgeMinimum,
        OrganizationId = source.OrganizationId == Guid.Empty ? null : source.OrganizationId,
        SchoolYearId = source.SchoolYearId == Guid.Empty ? null : source.SchoolYearId,
        SpaceId = source.SpaceId
    };

    /// <summary>Événement unique : champs communs et dates.</summary>
    public Ct.Event ApplyTo(Ct.Event target)
    {
        ApplyCommon(target);
        target.StartDate = Format.ParisToUtc(StartDate!.Value);
        target.EndDate = Format.ParisToUtc(EndDate!.Value);
        return target;
    }

    /// <summary>Série : champs communs, fréquence et heures ; les dates viennent de l'année scolaire.</summary>
    public Ct.EventSeries ToSeries() => new()
    {
        Event = ApplyCommon(new Ct.Event()),
        Frequency = Repeat!.Value,
        Interval = Interval,
        DayOfWeek = Repeat switch
        {
            Ct.RecurrenceFrequency.Weekly => WeeklyDay,
            Ct.RecurrenceFrequency.MonthlyByWeekday => MonthWeekday,
            _ => null
        },
        DayOfMonth = Repeat == Ct.RecurrenceFrequency.MonthlyByDate ? MonthDay : null,
        WeekOfMonth = Repeat == Ct.RecurrenceFrequency.MonthlyByWeekday ? MonthRank : null,
        StartTime = TimeOnly.Parse(StartTime!),
        EndTime = TimeOnly.Parse(EndTime!),
        DuringTermTime = DuringTermTime,
        DuringSchoolHolidays = DuringSchoolHolidays
    };

    private Ct.Event ApplyCommon(Ct.Event target)
    {
        target.RGBColor = Colors.FromInput(Color);
        target.Code = Code!.Trim();
        target.Name = Name!.Trim();
        target.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
        target.AgeMinimum = AgeMinimum;
        target.OrganizationId = OrganizationId!.Value;
        target.SchoolYearId = SchoolYearId!.Value;
        target.SpaceId = SpaceId;
        return target;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Repeat is null)
        {
            if (StartDate is null)
            {
                yield return new ValidationResult("Le début est obligatoire.", [nameof(StartDate)]);
            }

            if (EndDate is null)
            {
                yield return new ValidationResult("La fin est obligatoire.", [nameof(EndDate)]);
            }

            if (StartDate is { } start && EndDate is { } end && end <= start)
            {
                yield return new ValidationResult("La fin doit suivre le début.", [nameof(EndDate)]);
            }

            yield break;
        }

        if (!TimeOnly.TryParse(StartTime, out _))
        {
            yield return new ValidationResult("L'heure de début est obligatoire.", [nameof(StartTime)]);
        }

        if (!TimeOnly.TryParse(EndTime, out _))
        {
            yield return new ValidationResult("L'heure de fin est obligatoire.", [nameof(EndTime)]);
        }

        if (!DuringTermTime && !DuringSchoolHolidays)
        {
            yield return new ValidationResult("Cochez au moins « hors vacances scolaires » ou « pendant les vacances scolaires ».", [nameof(DuringTermTime)]);
        }

        switch (Repeat)
        {
            case Ct.RecurrenceFrequency.Weekly when WeeklyDay is null:
                yield return new ValidationResult("Choisissez le jour de la semaine.", [nameof(WeeklyDay)]);
                break;
            case Ct.RecurrenceFrequency.MonthlyByDate when MonthDay is not (>= 1 and <= 31):
                yield return new ValidationResult("Choisissez le jour du mois (1 à 31).", [nameof(MonthDay)]);
                break;
            case Ct.RecurrenceFrequency.MonthlyByWeekday when MonthRank is null || MonthWeekday is null:
                yield return new ValidationResult("Choisissez la semaine du mois et le jour.", [nameof(MonthRank)]);
                break;
        }
    }
}

public sealed class ProjectModel : IValidatableObject
{
    [Required(ErrorMessage = "Le code est obligatoire.")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Range(0, 120, ErrorMessage = "L'âge minimum doit être compris entre 0 et 120.")]
    public int AgeMinimum { get; set; }

    // Facultatives : vides, le projet couvre toute l'année scolaire.
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "L'organisation est obligatoire.")]
    public Guid? OrganizationId { get; set; }

    [Required(ErrorMessage = "L'année scolaire est obligatoire.")]
    public Guid? SchoolYearId { get; set; }

    public static ProjectModel From(Ct.Project source) => new()
    {
        Code = source.Code,
        Name = source.Name,
        Description = source.Description,
        AgeMinimum = source.AgeMinimum,
        StartDate = Format.ParisDay(source.StartDate),
        EndDate = Format.ParisDay(source.EndDate),
        OrganizationId = source.OrganizationId == Guid.Empty ? null : source.OrganizationId,
        SchoolYearId = source.SchoolYearId == Guid.Empty ? null : source.SchoolYearId
    };

    public Ct.Project ApplyTo(Ct.Project target)
    {
        target.Code = Code!.Trim();
        target.Name = Name!.Trim();
        target.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
        target.AgeMinimum = AgeMinimum;
        target.StartDate = Format.DayToUtc(StartDate);
        target.EndDate = Format.DayToUtc(EndDate);
        target.OrganizationId = OrganizationId!.Value;
        target.SchoolYearId = SchoolYearId!.Value;
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

/// <summary>Champs communs d'une série d'événements, appliqués à plusieurs occurrences (chacune garde sa date).</summary>
public sealed class EventSeriesModel : IValidatableObject
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Range(0, 120, ErrorMessage = "L'âge minimum doit être compris entre 0 et 120.")]
    public int? AgeMinimum { get; set; }

    [Required(ErrorMessage = "L'organisation est obligatoire.")]
    public Guid? OrganizationId { get; set; }

    [Required(ErrorMessage = "L'année scolaire est obligatoire.")]
    public Guid? SchoolYearId { get; set; }

    public Guid? SpaceId { get; set; }

    [Required(ErrorMessage = "L'heure de début est obligatoire.")]
    public string? StartTime { get; set; }

    [Required(ErrorMessage = "L'heure de fin est obligatoire.")]
    public string? EndTime { get; set; }

    /// <summary>Préremplissage depuis une occurrence (heures de Paris).</summary>
    public static EventSeriesModel From(Ct.Event source) => new()
    {
        Name = source.Name,
        Description = source.Description,
        AgeMinimum = source.AgeMinimum,
        OrganizationId = source.OrganizationId == Guid.Empty ? null : source.OrganizationId,
        SchoolYearId = source.SchoolYearId == Guid.Empty ? null : source.SchoolYearId,
        SpaceId = source.SpaceId,
        StartTime = Format.ParisLocal(source.StartDate).ToString("HH:mm"),
        EndTime = Format.ParisLocal(source.EndDate).ToString("HH:mm")
    };

    /// <summary>Corps de la requête ; une heure de fin avant l'heure de début signifie le lendemain.</summary>
    public Ct.EventSeriesUpdate ToUpdate()
    {
        var start = TimeOnly.Parse(StartTime!);
        var duration = TimeOnly.Parse(EndTime!) - start;

        return new Ct.EventSeriesUpdate
        {
            Name = Name!.Trim(),
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
            AgeMinimum = AgeMinimum,
            OrganizationId = OrganizationId!.Value,
            SchoolYearId = SchoolYearId!.Value,
            SpaceId = SpaceId,
            StartTime = start,
            DurationMinutes = (int)duration.TotalMinutes
        };
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (!string.IsNullOrEmpty(StartTime) && !TimeOnly.TryParse(StartTime, out _))
        {
            yield return new ValidationResult("L'heure de début n'est pas valide.", [nameof(StartTime)]);
        }

        if (!string.IsNullOrEmpty(EndTime) && !TimeOnly.TryParse(EndTime, out _))
        {
            yield return new ValidationResult("L'heure de fin n'est pas valide.", [nameof(EndTime)]);
        }
    }
}
