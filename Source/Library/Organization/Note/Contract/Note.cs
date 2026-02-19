namespace Library.Contract;

public class Note : PrimaryKey
{
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string RGBBackgroundColor { get; set; } = "FFFFFFFF";
    public Guid MemberId { get; set; }
}
