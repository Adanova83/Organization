using System.Net;
using Web.Clients;

namespace Web.Models;

/// <summary>Aide au traitement des erreurs d'appel à l'API dans les pages.</summary>
public static class ApiErrors
{
    /// <summary>Erreur renvoyée par l'API, ou API injoignable.</summary>
    public static bool IsApi(Exception ex) => ex is ApiException or HttpRequestException;

    public static bool IsNotFound(Exception ex) => ex is ApiException { StatusCode: HttpStatusCode.NotFound };

    public static bool IsUnauthorized(Exception ex) => ex is ApiException { StatusCode: HttpStatusCode.Unauthorized };

    public static string Message(Exception ex) => ex switch
    {
        ApiException { StatusCode: HttpStatusCode.Forbidden } api
            => $"{api.Message} (permission manquante)",
        ApiException api => api.Message,
        _ => "L'API est injoignable."
    };
}
