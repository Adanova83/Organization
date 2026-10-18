namespace Library.Domain;

public class Contact : PrimaryKey
{
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}
