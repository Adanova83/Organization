namespace Library.Domain;

public class Application : PrimaryKey
{
    public string Name { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public ICollection<AccessEvent> AccessEvents { get; set; } = new List<AccessEvent>();
}
