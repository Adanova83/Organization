namespace Library.Repository;

/// <summary>Suppression refusée : l'élément est encore référencé par d'autres données (traduite en 409 par l'API).</summary>
public sealed class InUseException : Exception
{
    public InUseException(string message) : base(message)
    {
    }
}
