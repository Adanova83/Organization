namespace Library.Contract;

public class NoteLink : PrimaryKey
{
    public Guid NoteId { get; set; }
    public string LinkedType { get; set; } = default!;
    public Guid LinkedId { get; set; }
}
