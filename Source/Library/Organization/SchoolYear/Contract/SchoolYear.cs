namespace Library.Contract;

/// <summary>Année scolaire (ex. 2026-2027), commune à toutes les organisations : saisons, sessions, événements et projets s'y rattachent.</summary>
public class SchoolYear : PrimaryKey
{
    public string Code { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    /// <summary>Zone de vacances scolaires : A, B ou C (B pour le Var). Les vacances de la zone sont importées à la création.</summary>
    public string Zone { get; set; } = "B";
}
