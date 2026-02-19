namespace Library.Domain;

public class Chat : PrimaryKey
{
    public string? Title { get; set; }
    public bool IsGroup { get; set; } = false;
    public Active Active { get; set; } = default!;
}
