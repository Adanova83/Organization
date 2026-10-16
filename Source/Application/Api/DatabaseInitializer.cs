using Library.Data;
using Microsoft.EntityFrameworkCore;

namespace Api;

public static class DatabaseInitializer
{
    /// <summary>
    /// Applique les migrations en attente au démarrage (si Migrations:ApplyAtStartup = true),
    /// avec plusieurs tentatives, comme le faisait l'ancienne application Migration.
    /// </summary>
    public static async Task ApplyMigrationsAtStartupAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("Migrations:ApplyAtStartup"))
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var db = scope.ServiceProvider.GetRequiredService<FullDbContext>();

        const int maxRetryCount = 5;

        for (var attempt = 1; attempt <= maxRetryCount; attempt++)
        {
            try
            {
                logger.LogInformation("Checking for pending database migrations.");
                var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();

                if (pending.Count == 0)
                {
                    logger.LogInformation("No pending database migrations found.");
                }
                else
                {
                    logger.LogInformation("Applying {Count} database migration(s)...", pending.Count);
                    await db.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully.");
                }

                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Migration failed. Attempt {Attempt} of {Max}.", attempt, maxRetryCount);

                if (attempt == maxRetryCount)
                {
                    logger.LogError(ex, "Max retry attempts reached. Migration failed.");
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}
