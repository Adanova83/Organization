namespace Library.Domain;

public class Badge : PrimaryKey
{
    public string Identifier { get; set; } = null!;
    public Active Active { get; set; } = null!;
}
