using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Library.Entity;

[Owned]
public class Audit
{
    [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required] public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; } = null;
}
