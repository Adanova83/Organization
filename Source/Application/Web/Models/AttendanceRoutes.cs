namespace Web.Models;

/// <summary>Adresses des pages de présences d'un créneau d'atelier (pas d'appel pour les événements).</summary>
public static class AttendanceRoutes
{
    /// <summary>Séances d'un créneau.</summary>
    public static string Sessions(Guid slotId) => $"/attendance/{slotId}";

    /// <summary>Appel d'une séance.</summary>
    public static string Session(Guid slotId, DateOnly date) => $"/attendance/{slotId}/{date:yyyy-MM-dd}";

    /// <summary>Date d'une séance dans l'adresse (yyyy-MM-dd).</summary>
    public static DateOnly? ParseDate(string value)
        => DateOnly.TryParseExact(value, "yyyy-MM-dd", out var date) ? date : null;
}
