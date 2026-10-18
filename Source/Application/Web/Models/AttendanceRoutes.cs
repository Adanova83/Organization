using Web.Clients;

namespace Web.Models;

/// <summary>Type d'élément dans l'adresse des pages de présences : « slot » (créneau d'atelier) ou « event ».</summary>
public static class AttendanceRoutes
{
    public static SubscriptionKind? Parse(string segment) => segment switch
    {
        "slot" => SubscriptionKind.Slot,
        "event" => SubscriptionKind.Event,
        _ => null
    };

    public static string Segment(SubscriptionKind kind) => kind == SubscriptionKind.Event ? "event" : "slot";

    /// <summary>Page des séances d'un créneau ou d'un événement.</summary>
    public static string Sessions(SubscriptionKind kind, Guid targetId) => $"/attendance/{Segment(kind)}/{targetId}";

    /// <summary>Date d'une séance dans l'adresse (yyyy-MM-dd).</summary>
    public static DateOnly? ParseDate(string value)
        => DateOnly.TryParseExact(value, "yyyy-MM-dd", out var date) ? date : null;
}
