using S = Application.Server.Settings;

namespace Application.Server;

public static class AppSettings
{
    private static IConfiguration _configuration = default!;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public static string GetAdanovaConnectionString() => GetConnectionString("Adanova");

    public static S.SmtpSetting GetSetting() => GetSection<S.SmtpSetting>("Settings:Smtp");

    private static string GetConnectionString(string name)
    {
        EnsureInitialized();
        var conn = _configuration.GetConnectionString(name);
        if(string.IsNullOrWhiteSpace(conn))
            throw new KeyNotFoundException($"Connection string '{name} can not be found.");
        return conn;
    }

    private static string GetString(string key)
        => _configuration[key]
           ?? throw new InvalidOperationException($"Configuration key '{key}' not found.");

    private static int GetInt(string key)
        => int.Parse(GetString(key));

    private static bool GetBool(string key)
        => bool.Parse(GetString(key));

    private static T GetSection<T>(string sectionName) where T : new()
    {
        var section = new T();
        _configuration.GetSection(sectionName).Bind(section);
        return section;
    }

    private static void EnsureInitialized()
    {
        if(_configuration == null)
            throw new InvalidOperationException("AppSettings has not been initialized. Call AppSettings.Init(Configuration) at startup.");
    }
}