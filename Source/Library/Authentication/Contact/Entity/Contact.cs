using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Contact", Schema = "Authentication")]
internal class Contact : PrimaryKey
{
    [MaxLength(200)] public string? Firstname { get; set; }
    [Required, MaxLength(200)] public string Lastname { get; set; } = null!;
    [MaxLength(50)] public string? PhoneNumber { get; set; }
    [MaxLength(320)] public string? Email { get; set; }
}
