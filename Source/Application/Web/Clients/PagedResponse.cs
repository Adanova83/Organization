namespace Web.Clients;

/// <summary>Réponse paginée de l'API : éléments de la page, nombre après filtre et nombre total.</summary>
public sealed record PagedResponse<T>(IReadOnlyList<T> Data, int FilteredCount, int TotalCount);
