using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Role", Schema = "Authentication")]
public class Role : PrimaryKey
{
    [Required, MaxLength(200)] public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    [Required, MaxLength(8)] public string RGBColor { get; set; } = "FFFFFFFF";
    public DateTime? Birthdate { get; set; } = null;
    [Required, MaxLength(200)] public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    [Required] public Address Addr { get; set; }  = default!;
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
