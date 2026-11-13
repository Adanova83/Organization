using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

/// <summary>
/// Présence d'un membre à une séance d'atelier : un créneau à une date donnée. Une seule présence par membre et par
/// séance. (Pas d'appel pour les événements.)
/// </summary>
[Table("Attendance", Schema = "Organization")]
[Index(nameof(SessionSlotId), nameof(Date), nameof(MemberId), IsUnique = true)]
internal class Attendance : PrimaryKey
{
    [Required] public Guid SessionSlotId { get; set; }
    [ForeignKey(nameof(SessionSlotId))] public ActivitySessionSlot SessionSlot { get; set; } = default!;
    /// <summary>Jour de la séance (heure de Paris).</summary>
    [Required] public DateOnly Date { get; set; }
    [Required] public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))] public Member Member { get; set; } = default!;
    [Required] public Contract.AttendanceStatus Status { get; set; }
    [Required] public Contract.AttendanceMethod Method { get; set; }
    [MaxLength(500)] public string? Comment { get; set; }
}
