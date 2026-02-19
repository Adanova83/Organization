using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Application", Schema = "Access")]
internal class Application : PrimaryKey
{
    [Required, MaxLength(100)] public string Name { get; set; } = null!;
    [Required, MaxLength(200)] public string Location { get; set; } = null!;
    [Required] public string ApiKey { get; set; } = null!;
    public ICollection<AccessEvent> AccessEvents { get; set; } = new List<AccessEvent>();
    public Audit App { get; set; } = null!;
    public Active Active { get; set; } = null!;
}
