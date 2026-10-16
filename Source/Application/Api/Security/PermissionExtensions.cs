namespace Api.Security;

public static class PermissionExtensions
{
    public static IServiceCollection AddPermissions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PermissionOptions>(configuration.GetSection(PermissionOptions.Section));
        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.Section));
        services.AddScoped<PermissionService>();
        services.AddScoped<CurrentMember>();
        return services;
    }

    /// <summary>
    /// Vrai si la requête porte la clé d'administration (en-tête X-Api-Key, clé configurée obligatoire).
    /// La clé d'API donne tous les droits : elle sert à l'administration et au développement,
    /// les utilisateurs passent par leur jeton OIDC. Elle n'est acceptée sur les endpoints métier
    /// que si Api:AllowKeyOnBusinessEndpoints vaut true (à mettre à false en production).
    /// </summary>
    public static bool HasValidApiKey(this HttpContext http)
    {
        var configuration = http.RequestServices.GetRequiredService<IConfiguration>();
        if (!configuration.GetValue<bool>("Api:AllowKeyOnBusinessEndpoints"))
        {
            return false;
        }

        var expected = configuration["Api:Key"];

        return !string.IsNullOrWhiteSpace(expected)
            && http.Request.Headers.TryGetValue(ApiKeyFilter.HeaderName, out var provided)
            && ApiKeyFilter.SecureEquals(provided.ToString(), expected);
    }

    /// <summary>Exige un utilisateur authentifié (jeton OIDC) ou la clé d'API. Sinon 401.</summary>
    public static TBuilder RequireAccess<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var http = context.HttpContext;

            if (http.User.Identity?.IsAuthenticated != true && !http.HasValidApiKey())
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Authentication required.",
                    detail: $"Send a bearer token, or the {ApiKeyFilter.HeaderName} header.");
            }

            return await next(context);
        });
    }

    /// <summary>
    /// Exige une permission (déduite des groupes du jeton), ou la clé d'API.
    /// Sans authentification : 401 ; authentifié mais sans la permission : 403.
    /// </summary>
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, string permission)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var http = context.HttpContext;

            if (http.HasValidApiKey())
            {
                return await next(context);
            }

            if (http.User.Identity?.IsAuthenticated != true)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Authentication required.",
                    detail: $"Send a bearer token, or the {ApiKeyFilter.HeaderName} header.");
            }

            var permissions = http.RequestServices.GetRequiredService<PermissionService>();
            if (!permissions.HasPermission(http.User, permission))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Missing permission.",
                    detail: $"The permission '{permission}' is required.");
            }

            return await next(context);
        });
    }
}
