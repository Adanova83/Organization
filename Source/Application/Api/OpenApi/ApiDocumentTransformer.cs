using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Api.OpenApi;

/// <summary>
/// Complète le document OpenAPI : titre, et les deux moyens d'authentification de l'API
/// (jeton OIDC « Bearer » et clé d'API « X-Api-Key »), pour que le bouton Authorize de Swagger UI fonctionne.
/// </summary>
public sealed class ApiDocumentTransformer : IOpenApiDocumentTransformer
{
    private const string BearerScheme = "Bearer";
    private const string ApiKeyScheme = "ApiKey";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Organizations API",
            Version = "v1",
            Description = "API de l'extranet de l'organisation. Authentification : jeton OIDC (Authorization: Bearer ...) "
                + "ou clé d'API (en-tête X-Api-Key, développement uniquement)."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes[BearerScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Jeton d'accès OIDC émis par l'annuaire (Authentik)."
        };

        document.Components.SecuritySchemes[ApiKeyScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-Api-Key",
            Description = "Clé d'administration (API_KEY). Acceptée sur les endpoints métier seulement si Api:AllowKeyOnBusinessEndpoints est vrai."
        };

        // Deux exigences distinctes = l'un OU l'autre.
        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(BearerScheme, document)] = new List<string>()
        });
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(ApiKeyScheme, document)] = new List<string>()
        });

        return Task.CompletedTask;
    }
}
