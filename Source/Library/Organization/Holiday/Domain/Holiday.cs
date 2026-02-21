namespace Library.Domain;

public class Holiday : PrimaryKey
{
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
