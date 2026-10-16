using System.Security.Cryptography;
using System.Text;

namespace Api.Security;

/// <summary>
/// Protège les endpoints par une clé d'API envoyée dans l'en-tête X-Api-Key.
/// La clé attendue vient de la configuration (Api:Key) ou de la variable d'environnement API_KEY.
/// Si aucune clé n'est configurée : accès libre en Development, erreur 503 sinon.
/// </summary>
public sealed class ApiKeyFilter : IEndpointFilter
{
    public const string HeaderName = "X-Api-Key";

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public ApiKeyFilter(IConfiguration configuration, IHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var expected = _configuration["Api:Key"];

        if (string.IsNullOrWhiteSpace(expected))
        {
            if (_environment.IsDevelopment())
            {
                return await next(context);
            }

            return Results.Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "API key is not configured.");
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided)
            || !SecureEquals(provided.ToString(), expected))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: $"Missing or invalid {HeaderName} header.");
        }

        return await next(context);
    }

    internal static bool SecureEquals(string a, string b)
    {
        var hashA = SHA256.HashData(Encoding.UTF8.GetBytes(a));
        var hashB = SHA256.HashData(Encoding.UTF8.GetBytes(b));
        return CryptographicOperations.FixedTimeEquals(hashA, hashB);
    }
}
