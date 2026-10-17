using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Api.Crud;

/// <summary>
/// Traduit les erreurs de validation de la bibliothèque (ArgumentException) en 400,
/// et les conflits de données (clé étrangère, unicité) en 409, au lieu d'une erreur 500.
/// </summary>
public sealed class DomainExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problems;

    public DomainExceptionHandler(IProblemDetailsService problems)
    {
        _problems = problems;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        int status;
        string title;
        string? detail = null;

        switch (exception)
        {
            case ArgumentException argument:
                status = StatusCodes.Status400BadRequest;
                title = "Invalid request.";
                detail = argument.Message;
                break;
            case Library.Domain.SubscriptionRefusedException refused:
                status = StatusCodes.Status409Conflict;
                // Message destiné aux membres (affiché tel quel par le site) : en français.
                title = "Inscription refusée.";
                detail = refused.Message;
                break;
            case Library.Repository.InUseException inUse:
                status = StatusCodes.Status409Conflict;
                // Message destiné aux utilisateurs (politique de suppression, FullDbContextDeletion) : en français.
                title = "Suppression impossible.";
                detail = inUse.Message;
                break;
            case DbUpdateException:
                status = StatusCodes.Status409Conflict;
                title = "The change conflicts with existing data.";
                detail = "A referenced item does not exist, is still in use, or a value that must be unique already exists.";
                break;
            default:
                return false;
        }

        context.Response.StatusCode = status;
        return await _problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = { Status = status, Title = title, Detail = detail }
        });
    }
}
