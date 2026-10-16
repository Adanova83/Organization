using System.Text.RegularExpressions;

namespace Api;

public static partial class ConnectionStrings
{
    /// <summary>
    /// Lit la chaîne de connexion et remplace les placeholders ${VARIABLE} par les variables d'environnement
    /// (DB_HOST, DB_PORT, DB_NAME, DB_USER, DB_PASSWORD).
    /// </summary>
    public static string Get(IConfiguration configuration, string name)
    {
        var raw = configuration.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidOperationException($"Connection string '{name}' not found.");
        }

        return Placeholder().Replace(raw, m => Environment.GetEnvironmentVariable(m.Groups[1].Value) ?? string.Empty);
    }

    [GeneratedRegex(@"\$\{(\w+)\}")]
    private static partial Regex Placeholder();
}
