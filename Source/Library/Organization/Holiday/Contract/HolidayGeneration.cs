namespace Library.Contract;

/// <summary>Résultat de la génération des jours fériés d'une année : jours créés, et noms de ceux qui existaient déjà.</summary>
public class HolidayGeneration
{
    public int Year { get; set; }
    public List<Holiday> Created { get; set; } = [];
    public List<string> Skipped { get; set; } = [];
}
