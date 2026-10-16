using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Library.Data;

/// <summary>
/// Utilisée uniquement par les outils EF (dotnet ef migrations ...).
/// Chaîne de connexion : variable ADANOVA_CONNECTION_STRING, sinon variables DB_HOST/DB_PORT/DB_NAME/DB_USER/DB_PASSWORD
/// (les mêmes que l'application Migration). Aucun identifiant n'est stocké dans le code.
/// </summary>
public class FullDbContextFactory
    : IDesignTimeDbContextFactory<FullDbContext>
{
    public FullDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FullDbContext>();

        // Même table d'historique que l'API (Program.cs), sinon les outils rejoueraient toutes les migrations.
        optionsBuilder.UseNpgsql(GetConnectionString(), npgsql => npgsql.MigrationsHistoryTable("MigrationsHistory", "Migration"));

        return new FullDbContext(optionsBuilder.Options);
    }

    private static string GetConnectionString()
    {
        var full = Environment.GetEnvironmentVariable("ADANOVA_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(full))
        {
            return full;
        }

        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var name = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(user))
        {
            throw new InvalidOperationException(
                "Chaîne de connexion introuvable pour les outils EF : définissez ADANOVA_CONNECTION_STRING " +
                "ou DB_HOST, DB_PORT, DB_NAME, DB_USER et DB_PASSWORD.");
        }

        return $"Host={host};Port={(string.IsNullOrWhiteSpace(port) ? "5432" : port)};Database={name};Username={user};Password={password}";
    }
}
