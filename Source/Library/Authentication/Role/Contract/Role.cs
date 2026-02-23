namespace Library.Contract;

public sealed class Role : PrimaryKey
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
}
