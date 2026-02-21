namespace Library.Contract;

public class Document : PrimaryKey
{
    public string Title { get; set; } = default!;
    public Guid MemberId { get; set; }
}
