using Library.Domain;

namespace Web.Models;

/// <summary>
/// Pré-contrôle d'âge des boutons « S'inscrire », avec la même règle que l'API (<see cref="SubscriptionRules"/>) :
/// âge apprécié au début de l'activité, date de naissance requise. L'API reste seule juge (capacité, doublons...).
/// </summary>
public static class SubscriptionHints
{
    /// <summary>Raison d'empêcher l'inscription, ou null si rien ne s'y oppose côté âge.</summary>
    public static string? AgeBlock(int? ageMinimum, DateOnly? birthdate, DateTime? startUtc)
    {
        if (ageMinimum is not > 0)
        {
            return null;
        }

        if (birthdate is not { } born)
        {
            return $"Âge minimum : {ageMinimum} ans (date de naissance à renseigner dans votre profil)";
        }

        var today = DateOnly.FromDateTime(Format.ParisLocal(DateTime.UtcNow));
        var start = startUtc is { } value ? DateOnly.FromDateTime(Format.ParisLocal(value)) : (DateOnly?)null;
        return SubscriptionRules.AgeOn(born, SubscriptionRules.AgeReference(start, today)) < ageMinimum
            ? $"Âge minimum : {ageMinimum} ans"
            : null;
    }
}
