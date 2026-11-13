namespace Library.Contract;

/// <summary>Contact d'une famille (parent, tuteur...) : fiche d'information gérée par les responsables, sans compte.</summary>
public class Contact : PrimaryKey
{
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}
