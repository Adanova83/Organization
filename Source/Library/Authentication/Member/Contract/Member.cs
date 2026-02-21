namespace Library.Contract;

public sealed class Member : PrimaryKey
{
    public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public DateTime? Birthdate { get; set; } = null;
    public string Email { get; set; } = default!;
    public string HashedPassword { get; set; } = default!;
    public Guid? AskedNewPassword { get; set; }
    public string? PhoneNumber { get; set; }
    public Address Addr { get; set; } = default!;
}
