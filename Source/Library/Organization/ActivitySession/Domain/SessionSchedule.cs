namespace Library.Domain;

/// <summary>Période sans activité ordinaire : vacances scolaires (rattachées à une année scolaire) ou jour férié.</summary>
public sealed record ClosedPeriod(DateOnly Start, DateOnly End, bool IsSchoolHoliday)
{
    public bool Contains(DateOnly date) => Start <= date && date <= End;
}

/// <summary>
/// Règles de déroulement d'une session d'activité selon le calendrier scolaire :
/// jamais les jours fériés ; pendant les vacances scolaires seulement si la session s'y déroule ;
/// en période scolaire seulement si la session s'y déroule.
/// </summary>
public static class SessionSchedule
{
    public static bool IsActiveOn(DateOnly date, bool duringTermTime, bool duringSchoolHolidays, IEnumerable<ClosedPeriod> closedPeriods)
    {
        var periods = closedPeriods as IReadOnlyCollection<ClosedPeriod> ?? closedPeriods.ToList();

        if (periods.Any(p => !p.IsSchoolHoliday && p.Contains(date)))
        {
            return false;
        }

        return periods.Any(p => p.IsSchoolHoliday && p.Contains(date)) ? duringSchoolHolidays : duringTermTime;
    }

    /// <summary>Libellé court du rythme d'une session.</summary>
    public static string Label(bool duringTermTime, bool duringSchoolHolidays) => (duringTermTime, duringSchoolHolidays) switch
    {
        (true, true) => "Toute l'année",
        (false, true) => "Vacances scolaires",
        _ => "Hors vacances scolaires"
    };
}
