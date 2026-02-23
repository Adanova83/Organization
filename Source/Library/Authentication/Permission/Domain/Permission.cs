namespace Library.Domain;

public sealed class Permission : PrimaryKey
{
    public string Type { get; set; } = default!;
    public string Value { get; set; } = default!;
}
