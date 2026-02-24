namespace Library.Domain;

public sealed class Member : PrimaryKey
{
    public string? Firstname { get; set; } = default;
    public string Lastname { get; set; } = default!;
    public string Initials { get
        {
            List<char> result = new List<char>();
            if(!string.IsNullOrEmpty(Firstname))
            {
                result.AddRange(Firstname.Replace('-', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(item => item[0]));
            }
            if(!string.IsNullOrEmpty(Lastname))
            {
                result.AddRange(Lastname.Replace('-', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(item => item[0]));
            }
            return string.Join("", result.Take(3));
        }
    }
    public string? Picture { get; set; }
    public string RGBColor { get; set; } = "FFFFFFFF";
    public DateTime? Birthdate { get; set; } = null;
    public string Email { get; set; } = default!;
    public string HashedPassword { get; set; } = default!;
    public Guid? AskedNewPassword { get; set; }
    public string? PhoneNumber { get; set; }
    public Address Addr { get; set; } = default!;
}
