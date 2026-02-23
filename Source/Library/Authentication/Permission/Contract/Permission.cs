namespace Library.Contract;

public sealed class Permission : PrimaryKey
{
    public string Type { get; set; } = default!;
    public string Value { get; set; } = default!;
}
