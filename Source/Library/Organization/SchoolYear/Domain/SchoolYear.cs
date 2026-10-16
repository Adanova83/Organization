namespace Library.Domain;

public class SchoolYear : PrimaryKey
{
    public string Code { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Zone { get; set; } = "B";

    public static readonly IReadOnlyList<string> Zones = ["A", "B", "C"];
}
