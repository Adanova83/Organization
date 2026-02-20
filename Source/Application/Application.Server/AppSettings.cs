namespace Application.Server;

public static class AppSettings
{
    private static readonly IConfigurationRoot _configuration;

    static AppSettings()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static string GetString(string key)
        => _configuration[key]
           ?? throw new InvalidOperationException($"Configuration key '{key}' not found.");

    public static int GetInt(string key)
        => int.Parse(GetString(key));

    public static bool GetBool(string key)
        => bool.Parse(GetString(key));

    public static T GetSection<T>(string sectionName) where T : new()
    {
        var section = new T();
        _configuration.GetSection(sectionName).Bind(section);
        return section;
    }
}