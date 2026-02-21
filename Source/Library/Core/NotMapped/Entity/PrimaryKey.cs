using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[NotMapped]
public abstract class PrimaryKey
{
    [Key] public Guid Id { get; set; } = Guid.Empty;
    public Active Active { get; set; } = default!;
    public Audit Audit { get; set; } = default!;
}
