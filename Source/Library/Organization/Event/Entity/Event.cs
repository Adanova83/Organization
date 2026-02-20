using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Event", Schema = "Organization")]
[Index(nameof(Code), IsUnique = true)]
internal class Event : PrimaryKey
{
    [Required, MaxLength(50)] public string Code { get; set; } = default!;
    [Required, MaxLength(100)] public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Picture { get; set; }
    [Required, MaxLength(8)] public string RGBColor { get; set; } = "FFFFFFFF";
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    public int? AgeMinimum { get; set; } = null;
    [Required] public Guid OrganizationSessionId { get; set; }
    [ForeignKey(nameof(OrganizationSessionId))] public OrganizationSession OrganizationSession { get; set; } = default!;
    public Guid? SpaceId { get; set; }
    [ForeignKey(nameof(SpaceId))] public Space? Space { get; set; } = null;
    public Audit Audit { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
