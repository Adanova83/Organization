namespace Library.Domain;

public sealed class Role : PrimaryKey
{
    public string Type { get; set; } = default!;
    public string Value { get; set; } = default!;
}
