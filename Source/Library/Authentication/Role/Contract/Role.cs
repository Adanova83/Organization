namespace Library.Contract;

public sealed class Role : PrimaryKey
{
    public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public DateTime? Birthdate { get; set; } = null;
    public Address Addr { get; set; } = new();
    public string Email { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
