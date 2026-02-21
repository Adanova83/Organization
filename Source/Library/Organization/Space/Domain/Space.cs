namespace Library.Domain;

public class Space : PrimaryKey
{
    public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
}
