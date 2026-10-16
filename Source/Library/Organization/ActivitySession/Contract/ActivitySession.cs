namespace Library.Contract;

/// <summary>
/// Session d'une activité pour une année scolaire. Par défaut elle couvre toute l'année ; StartDate / EndDate,
/// facultatives, la restreignent (stage de mars à juin...).
/// </summary>
public class ActivitySession : PrimaryKey
{
    public Guid ActivityId { get; set; }
    public Guid SchoolYearId { get; set; }
    public float Price { get; set; } = 0.0f;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Rythme : en période scolaire (hors vacances) et/ou pendant les vacances scolaires (au moins l'un des deux). Jamais les jours fériés.
    public bool DuringTermTime { get; set; } = true;
    public bool DuringSchoolHolidays { get; set; }

    // Renseignés par l'API (lecture seule, ignorés à l'envoi) : année scolaire et dates effectives (propres, sinon celles de l'année).
    public string? SchoolYearCode { get; set; }
    public DateTime? EffectiveStartDate { get; set; }
    public DateTime? EffectiveEndDate { get; set; }
}
