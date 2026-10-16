namespace Library.Domain;

/// <summary>Inscription refusée par une règle métier (doublon, complet, âge) : traduite en 409 par l'API, avec un message lisible.</summary>
public sealed class SubscriptionRefusedException : Exception
{
    public SubscriptionRefusedException(string message) : base(message)
    {
    }
}

/// <summary>
/// Règles d'inscription aux créneaux d'ateliers, événements et créneaux de projets :
/// jamais deux fois le même membre ; capacité et âge minimum, auxquels un responsable (subscription.edit) peut déroger.
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

    /// <param name="others">Membres déjà inscrits (hors l'inscription modifiée).</param>
    /// <param name="canOverride">Responsable : capacité et âge ne s'appliquent pas (les doublons, si).</param>
    public static void Check(
        Guid memberId, IReadOnlyCollection<Guid> others, int? capacity,
        int ageMinimum, DateOnly? birthdate, DateOnly ageReference, bool canOverride)
    {
        if (others.Contains(memberId))
        {
            throw new SubscriptionRefusedException("Ce membre est déjà inscrit.");
        }

        if (canOverride)
        {
            return;
        }

        if (capacity is { } places && others.Count >= places)
        {
            throw new SubscriptionRefusedException($"Complet : les {places} place(s) sont prises.");
        }

        if (ageMinimum <= 0)
        {
            return;
        }

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
}
