namespace Library.Domain;

public class Holiday : PrimaryKey
{
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Année scolaire, facultative : renseignée pour les vacances scolaires, vide pour les jours fériés (année civile).
    public Guid? SchoolYearId { get; set; }
}
