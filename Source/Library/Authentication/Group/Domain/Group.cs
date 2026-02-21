namespace Library.Domain;

public sealed class Group : PrimaryKey
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
