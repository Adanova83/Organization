using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Library.Entity;

[Owned]
public class Active
{
    [Required] public DateTime From { get; set; } = DateTime.UtcNow;
    public DateTime? Until { get; set; } = null;
}