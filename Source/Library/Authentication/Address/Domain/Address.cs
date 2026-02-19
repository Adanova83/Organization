namespace Library.Domain;

public class Address
{
    public string Line1 { get; set; } = default!;
    public string? Line2 { get; set; } = null;
    public string? Line3 { get; set; } = null;
    public string City { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public Guid CountryId { get; set; }
    
    public Address() { }

    public Address(string line1, string city, string postalCode)
        : this(line1, null, null, city, postalCode)
    { }

    public Address(string line1, string line2, string city, string postalCode)
        : this(line1, line2, null, city, postalCode)
    { }

    public Address(string line1, string? line2, string? line3, string city, string postalCode)
    {
        string error = CheckData(line1, line2, line3, city, postalCode);
        if (error != string.Empty)
        {
            throw new ArgumentException(error);
        }

        Line1 = line1;
        Line2 = line2;
        Line3 = line3;
        City = city;
        PostalCode = postalCode;
    }

    private string CheckData(string line1, string? line2, string? line3, string city, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(line1))
        {
            return "Address line 1 cannot be empty.";
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return "City cannot be empty.";
        }

        if (string.IsNullOrWhiteSpace(postalCode))
        {
            return "Postal code cannot be empty.";
        }

        return string.Empty;
    }
}
