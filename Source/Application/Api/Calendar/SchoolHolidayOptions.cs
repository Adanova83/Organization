namespace Api.Calendar;

/// <summary>
/// Source des vacances scolaires : jeu de données « Calendrier scolaire » de l'Éducation nationale (data.education.gouv.fr).
/// La zone (A, B ou C) est celle de l'année scolaire.
/// </summary>
public sealed class SchoolHolidayOptions
{
    public const string Section = "SchoolHolidays";

    public string DatasetUrl { get; set; } =
        "https://data.education.gouv.fr/api/explore/v2.1/catalog/datasets/fr-en-calendrier-scolaire/records";
}
