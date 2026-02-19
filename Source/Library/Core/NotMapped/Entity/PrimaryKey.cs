using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[NotMapped]
public abstract class PrimaryKey
{
    [Key] public Guid Id { get; set; } = Guid.Empty;
}
