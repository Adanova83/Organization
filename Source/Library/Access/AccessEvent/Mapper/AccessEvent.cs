using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class AccessEvent
{
    public static Ct.AccessEvent DomainToContract(this Do.AccessEvent source)
    {
        return new Ct.AccessEvent
        {
            Id = source.Id,
            BadgeId = source.BadgeId,
            ApplicationId = source.ApplicationId,
            AccessedAt = source.AccessedAt,
            ExitedAt = source.ExitedAt,
            IsAuthorized = source.IsAuthorized
        };
    }

    internal static En.AccessEvent DomainToEntity(this Do.AccessEvent source)
    {
        return new En.AccessEvent
        {
            Id = source.Id,
            BadgeId = source.BadgeId,
            ApplicationId = source.ApplicationId,
            AccessedAt = source.AccessedAt,
            ExitedAt = source.ExitedAt,
            IsAuthorized = source.IsAuthorized
        };
    }

    internal static Do.AccessEvent EntityToDomain(this En.AccessEvent source)
    {
        return new Do.AccessEvent
        {
            Id = source.Id,
            BadgeId = source.BadgeId,
            ApplicationId = source.ApplicationId,
            AccessedAt = source.AccessedAt,
            ExitedAt = source.ExitedAt,
            IsAuthorized = source.IsAuthorized
        };
    }
}
