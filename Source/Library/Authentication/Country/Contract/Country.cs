namespace Library.Contract;

public class Country : PrimaryKey
{
    public string CodeAlpha2 { get; set; } = default!;
    public string CodeAlpha3 { get; set; } = default!;
    public string Name { get; set; } = default!;
}
