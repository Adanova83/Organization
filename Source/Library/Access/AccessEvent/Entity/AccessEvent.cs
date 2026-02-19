using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("AccessEvent", Schema = "Access")]
internal class AccessEvent : PrimaryKey
{
    [Required] public Guid BadgeId { get; set; }
    [ForeignKey(nameof(BadgeId))] public Badge Badge { get; set; } = null!;
    [Required] public Guid ApplicationId { get; set; }
    [ForeignKey(nameof(ApplicationId))] public Application Application { get; set; } = null!;
    public DateTime AccessedAt { get; set; }
    public DateTime? ExitedAt { get; set; }
    public bool IsAuthorized { get; set; }
    public Audit Aev { get; set; } = null!;
}
