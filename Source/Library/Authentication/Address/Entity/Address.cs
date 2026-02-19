using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Library.Entity;

[Owned]
public class Address
{
    [Required, MaxLength(200)] public string Line1 { get; set; } = default!;
    [MaxLength(200)] public string? Line2 { get; set; } = null;
    [MaxLength(200)] public string? Line3 { get; set; } = null;
    [Required, MaxLength(200)] public string City { get; set; } = default!;
    [Required, MaxLength(20)] public string PostalCode { get; set; } = default!;
    [Required] public Guid CountryId { get; set; }
    [ForeignKey(nameof(CountryId))] public Country Country { get; set; } = default!;

}
