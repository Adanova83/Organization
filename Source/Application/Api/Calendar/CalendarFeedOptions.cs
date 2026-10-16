namespace Api.Calendar;

/// <summary>
/// Flux iCalendar (ICS) du planning, auxquels Nextcloud s'abonne (Agenda, « Nouvel abonnement »).
/// Les flux sont désactivés tant que FeedToken n'est pas renseigné.
/// </summary>
public sealed class CalendarFeedOptions
{
    public const string Section = "Calendar";

    /// <summary>
    /// Jeton secret passé dans l'URL du flux (?token=...). Nextcloud ne sait pas envoyer d'en-tête
    /// personnalisé lors d'un abonnement : le jeton fait donc partie de l'adresse, à ne pas diffuser.
    /// </summary>
    public string? FeedToken { get; set; }

    /// <summary>Suffixe des identifiants (UID) iCalendar : type-guid@UidDomain.</summary>
    public string UidDomain { get; set; } = "organizations.local";

    /// <summary>
    /// Convention du numéro de jour des créneaux (0 à 6) : false = comme System.DayOfWeek (0 = dimanche),
    /// true = 0 = lundi.
    /// </summary>
    public bool DayOfWeekStartsOnMonday { get; set; }

    public bool IsEnabled => !string.IsNullOrWhiteSpace(FeedToken);
}
