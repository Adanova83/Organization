using System.Globalization;
using Ct = Library.Contract;

namespace Web.Models;

/// <summary>Mise en forme des dates, heures et prix pour l'affichage (heure de Paris, format français).</summary>
public static class Format
{
    private static readonly CultureInfo French = new("fr-FR");
    private static readonly TimeZoneInfo Paris = FindParis();

    // Convention de l'API : jour 0 = dimanche.
    private static readonly string[] Days = ["Dimanche", "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi"];

    public static string Day(int dayOfWeek) => dayOfWeek is >= 0 and <= 6 ? Days[dayOfWeek] : $"Jour {dayOfWeek}";

    public static string Date(DateTime value) => ToParis(value).ToString("dd/MM/yyyy", French);

    public static string Date(DateOnly value) => value.ToString("dd/MM/yyyy", French);

    /// <summary>Places d'un créneau ou d'un événement : « 8 / 12 », « Complet », avec la liste d'attente ; capacité seule si les inscrits sont inconnus.</summary>
    public static string Places(int? capacity, Ct.SubscriptionAvailability? availability)
    {
        if (availability is null)
        {
            return capacity is { } total ? $"{total} place(s)" : "Illimitées";
        }

        var waiting = availability.Waiting > 0 ? $" · {availability.Waiting} en attente" : "";
        return availability.Capacity is { } places
            ? (availability.IsFull ? $"Complet ({availability.Subscribed} / {places}){waiting}" : $"{availability.Subscribed} / {places}{waiting}")
            : $"{availability.Subscribed} inscrit(s){waiting}";
    }

    /// <summary>Période « du … au … » ; « — » si une date manque.</summary>
    public static string Period(DateTime? start, DateTime? end)
        => start is { } s && end is { } e ? $"Du {Date(s)} au {Date(e)}" : "—";

    public static string DateTimeLabel(DateTime value) => ToParis(value).ToString("dd/MM/yyyy HH:mm", French);

    public static string Time(TimeOnly value) => value.ToString("HH:mm", French);

    public static string Price(float value) => value == 0 ? "Gratuit" : value.ToString("C2", French);

    /// <summary>Montant (payé, reste...) : « 0,00 € » plutôt que « Gratuit ».</summary>
    public static string Money(float value) => value.ToString("C2", French);

    public static string Age(int? minimum) => minimum is null or 0 ? "Tout âge" : $"{minimum} ans et +";

    public static string Frequency(int weeks) => weeks <= 1 ? "Chaque semaine" : $"Toutes les {weeks} semaines";

    /// <summary>Heure de Paris d'une date UTC, sans fuseau (pour les champs de formulaire).</summary>
    public static DateTime ParisLocal(DateTime utc)
        => DateTime.SpecifyKind(ToParis(utc), DateTimeKind.Unspecified);

    /// <summary>Jour (date de Paris, sans heure) d'une date UTC facultative, pour un champ de formulaire.</summary>
    public static DateTime? ParisDay(DateTime? utc) => utc is { } value ? ParisLocal(value).Date : null;

    /// <summary>Minuit de Paris (en UTC) d'un jour saisi dans un formulaire, ou null si le champ est vide.</summary>
    public static DateTime? DayToUtc(DateTime? day) => day is { } value ? ParisToUtc(value.Date) : null;

    /// <summary>Date UTC correspondant à une heure de Paris saisie dans un formulaire.</summary>
    public static DateTime ParisToUtc(DateTime local)
        => TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), Paris);

    /// <summary>Âge en années à une date donnée (aujourd'hui par défaut), ou null si la naissance est inconnue.</summary>
    public static int? AgeYears(DateTime? birthdate, DateTime? on = null)
    {
        if (birthdate is not { } born)
        {
            return null;
        }

        var birth = ToParis(born).Date;
        var reference = (on ?? DateTime.UtcNow);
        var today = ToParis(reference.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(reference, DateTimeKind.Utc) : reference).Date;
        var age = today.Year - birth.Year;
        return birth > today.AddYears(-age) ? age - 1 : age;
    }

    private static DateTime ToParis(DateTime value)
        => value.Kind == DateTimeKind.Utc ? TimeZoneInfo.ConvertTimeFromUtc(value, Paris) : value;

    private static TimeZoneInfo FindParis()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            // Image sans données de fuseaux horaires : on affiche l'heure UTC.
            return TimeZoneInfo.Utc;
        }
    }
}
