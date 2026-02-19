namespace Library.Domain;

public class AccessEvent : PrimaryKey
{
    public Guid BadgeId { get; set; } = Guid.Empty;
    public Guid ApplicationId { get; set; } = Guid.Empty;
    public DateTime AccessedAt { get; set; }
    public DateTime? ExitedAt { get; set; }
    public bool IsAuthorized { get; set; }
    public TimeSpan? Duration => ExitedAt.HasValue ? ExitedAt - AccessedAt : null;
}
