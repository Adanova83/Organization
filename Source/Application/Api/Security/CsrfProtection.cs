using Microsoft.Extensions.Options;

namespace Api.Security;

/// <summary>
/// Protection CSRF des requêtes qui modifient des données et s'appuient sur le cookie de session :
/// en plus de SameSite=Lax, l'en-tête <see cref="HeaderName"/> est obligatoire (un site tiers ne peut pas
/// l'ajouter à une requête inter-origines sans autorisation CORS, que l'API n'accorde pas).
/// Les requêtes portant un Bearer ou une clé d'API ne sont pas concernées : le navigateur ne les joint pas tout seul.
/// </summary>
public static class CsrfProtection
{
    public const string HeaderName = "X-Csrf";

    public static IApplicationBuilder UseCsrfProtection(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var request = context.Request;

            var safeMethod = HttpMethods.IsGet(request.Method)
                || HttpMethods.IsHead(request.Method)
                || HttpMethods.IsOptions(request.Method)
                || HttpMethods.IsTrace(request.Method);

            if (!safeMethod
                && request.Path.StartsWithSegments("/api")
                && !request.Headers.ContainsKey("Authorization")
                && !request.Headers.ContainsKey(ApiKeyFilter.HeaderName))
            {
                var cookieName = context.RequestServices.GetRequiredService<IOptions<WebSessionOptions>>().Value.CookieName;

                if (request.Cookies.ContainsKey(cookieName) && !request.Headers.ContainsKey(HeaderName))
                {
                    await Results.Problem(
                            statusCode: StatusCodes.Status403Forbidden,
                            title: "Missing CSRF header.",
                            detail: $"The {HeaderName} header is required on requests that modify data.")
                        .ExecuteAsync(context);
                    return;
                }
            }

            await next();
        });
    }
}
