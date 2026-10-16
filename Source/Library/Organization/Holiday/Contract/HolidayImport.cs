namespace Library.Contract;

/// <summary>Résultat de l'import des vacances scolaires d'une année : périodes créées, et noms de celles déjà présentes.</summary>
public class HolidayImport
{
    public string SchoolYearCode { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public List<Holiday> Created { get; set; } = [];
    public List<string> Skipped { get; set; } = [];
}
