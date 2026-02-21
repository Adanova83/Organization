namespace Library.Domain;

public class PrimaryKey
{
    public Guid Id { get; init; }
    public Active Active { get; set; } = default!;
    public Audit Audit { get; set; } = default!;
}
