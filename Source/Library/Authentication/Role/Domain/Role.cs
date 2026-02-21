namespace Library.Domain;

public sealed class Role : PrimaryKey
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
