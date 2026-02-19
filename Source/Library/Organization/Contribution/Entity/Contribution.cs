using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

[Table("Contribution", Schema = "Organization")]
internal class Contribution : PrimaryKey
{
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    [Required] public Guid OrganizationSessionId { get; set; }
    [ForeignKey(nameof(OrganizationSessionId))] public OrganizationSession OrganizationSession { get; set; } = default!;
    public DateOnly PayedDate { get; set; }
    public Audit Ctb { get; set; } = default!;
    public Active Active { get; set; } = default!;
}
