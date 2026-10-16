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

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.AccessEvent ContractToDomain(this Ct.AccessEvent source, Guid id)
    {
        return new Do.AccessEvent
        {
            Id = id,
            BadgeId = source.BadgeId,
            ApplicationId = source.ApplicationId,
            AccessedAt = source.AccessedAt,
            ExitedAt = source.ExitedAt,
            IsAuthorized = source.IsAuthorized,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
