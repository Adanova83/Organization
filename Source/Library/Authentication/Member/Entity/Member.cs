using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Member", Schema = "Authentication")]
[Index(nameof(Email), IsUnique = true)]
public class Member : PrimaryKey
{
    [Required, MaxLength(200)] public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    [Required, MaxLength(8)] public string RGBColor { get; set; } = "FFFFFFFF";
    public DateTime? Birthdate { get; set; } = null;
    [Required, MaxLength(320)] public string Email { get; set; } = default!;
    [Required, MaxLength(255)] public string HashedPassword { get; set; } = default!;
    public Guid? AskedNewPassword { get; set; }
    public string? PhoneNumber { get; set; }
    [Required] public Address Addr { get; set; } = default!;
}

