using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

/// <summary>
/// Présence d'un membre à une séance : un créneau d'atelier à une date donnée, ou un événement (une seule de ces deux
/// cibles est renseignée). Une seule présence par membre et par séance.
/// </summary>
[Table("Attendance", Schema = "Organization")]
[Index(nameof(SessionSlotId), nameof(Date), nameof(MemberId), IsUnique = true)]
[Index(nameof(EventId), nameof(MemberId), IsUnique = true)]
internal class Attendance : PrimaryKey
{
    public Guid? SessionSlotId { get; set; }
    [ForeignKey(nameof(SessionSlotId))] public ActivitySessionSlot? SessionSlot { get; set; }
    public Guid? EventId { get; set; }
    [ForeignKey(nameof(EventId))] public Event? Event { get; set; }
    /// <summary>Jour de la séance (heure de Paris).</summary>
    [Required] public DateOnly Date { get; set; }
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    [Required] public Contract.AttendanceStatus Status { get; set; }
    [Required] public Contract.AttendanceMethod Method { get; set; }
    [MaxLength(500)] public string? Comment { get; set; }
}
