namespace Library.Contract;

public class Application : PrimaryKey
{
    public string Name { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public Active Active { get; set; } = null!;
}
