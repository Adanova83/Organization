namespace Library.Domain;

/// <summary>Inscription refusée par une règle métier (doublon, âge) : traduite en 409 par l'API, avec un message lisible.</summary>
public sealed class SubscriptionRefusedException : Exception
{
    public SubscriptionRefusedException(string message) : base(message)
    {
    }
}

/// <summary>
/// Règles d'inscription aux créneaux d'ateliers, événements et créneaux de projets :
/// jamais deux fois le même membre (inscrit ou en attente) ; âge minimum ; élément complet : liste d'attente.
/// Un responsable (subscription.edit) peut déroger à l'âge et à la capacité (inscription directe).
/// </summary>
public static class SubscriptionRules
{
    /// <summary>Âge révolu à une date.</summary>
    public static int AgeOn(DateOnly birthdate, DateOnly on)
    {
        var age = on.Year - birthdate.Year;
        return birthdate > on.AddYears(-age) ? age - 1 : age;
    }

    /// <summary>Date à laquelle l'âge est apprécié : le début de l'activité, ou aujourd'hui si elle a déjà commencé.</summary>
    public static DateOnly AgeReference(DateOnly? start, DateOnly today) => start is { } first && first > today ? first : today;

    /// <param name="confirmed">Membres inscrits (hors l'inscription modifiée).</param>
    /// <param name="waiting">Membres sur liste d'attente (hors l'inscription modifiée).</param>
    /// <param name="canOverride">Responsable : âge et capacité ne s'appliquent pas (les doublons, si).</param>
    /// <returns>Vrai si l'inscription va en liste d'attente (élément complet).</returns>
    public static bool Check(
        Guid memberId, IReadOnlyCollection<Guid> confirmed, IReadOnlyCollection<Guid> waiting, int? capacity,
        int ageMinimum, DateOnly? birthdate, DateOnly ageReference, bool canOverride)
    {
        if (confirmed.Contains(memberId) || waiting.Contains(memberId))
        {
            throw new SubscriptionRefusedException("Ce membre est déjà inscrit ou sur liste d'attente.");
        }

        if (canOverride)
        {
            return false;
        }

        // L'âge est contrôlé avant la capacité : inutile de faire attendre un membre qui sera refusé.
        if (ageMinimum > 0)
        {
            if (birthdate is not { } born)
            {
                throw new SubscriptionRefusedException(
                    $"Âge minimum : {ageMinimum} ans. La date de naissance du membre est nécessaire : renseignez-la dans le profil.");
            }

            var age = AgeOn(born, ageReference);
            if (age < ageMinimum)
            {
                throw new SubscriptionRefusedException(
                    $"Âge minimum : {ageMinimum} ans (le membre aura {age} an(s) le {ageReference:dd/MM/yyyy}).");
            }
        }

        return capacity is { } places && confirmed.Count >= places;
    }
}
