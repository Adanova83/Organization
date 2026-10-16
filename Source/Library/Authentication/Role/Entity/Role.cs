using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entity;

/// <summary>
/// Présentation d'un rôle. Les rôles et leurs membres sont les groupes de l'annuaire (Authentik/LDAP) :
/// Code = nom du groupe. Cette table ne porte que ce que l'annuaire ne sait pas stocker (libellé, image, couleur).
/// </summary>
[Table("Role", Schema = "Authentication")]
[Index(nameof(Code), IsUnique = true)]
public class Role : PrimaryKey
{
    [Required, MaxLength(50)] public string Code { get; set; } = default!;
    [Required, MaxLength(200)] public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    [Required, MaxLength(8)] public string RGBColor { get; set; } = "FFFFFFFF";
}

