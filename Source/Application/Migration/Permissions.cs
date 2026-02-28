namespace Migration;

public static class Permissions
{
    private static readonly string[] _modules =
    [
        "activity",
        "application",
        "badge",
        "contribution",
        "document",
        "event",
        "member",
        "note",
        "organization",
        "parameter",
        "project",
    ];

    private static readonly string[] _actions =
    [
        "read",
        "write",
    ];

    public static readonly string[] All =
        _modules.SelectMany(m => _actions.Select(a => $"{m}.{a}")).ToArray();

    public static readonly string[] Read =
        _modules.SelectMany(m => _actions.Where(a => a == "read").Select(a => $"{m}.{a}")).ToArray();

    public static readonly string[] Write =
        _modules.SelectMany(m => _actions.Where(a => a == "write").Select(a => $"{m}.{a}")).ToArray();
}