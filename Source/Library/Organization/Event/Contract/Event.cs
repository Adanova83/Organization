namespace Library.Contract;

public class Event : PrimaryKey
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? AgeMinimum { get; set; } = null;
    public Guid OrganizationId { get; set; }
    public Guid SchoolYearId { get; set; }
    public Guid? SpaceId { get; set; }

    /// <summary>Nombre maximal d'inscrits ; vide = illimité.</summary>
    public int? Capacity { get; set; }

    // Série d'événements récurrents : même identifiant pour toutes les occurrences (null : événement isolé).
    public Guid? SeriesId { get; set; }

    // Code de l'année scolaire (lecture seule, renseigné à la lecture).
    public string? SchoolYearCode { get; set; }
}
