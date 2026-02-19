namespace Library.Domain;

public class Organization : PrimaryKey
{
    public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public Active Active { get; set; } = default!;
}
