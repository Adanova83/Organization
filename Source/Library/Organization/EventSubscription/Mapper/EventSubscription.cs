using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class EventSubscription
{
    public static Ct.EventSubscription DomainToContract(this Do.EventSubscription source)
    {
        return new Ct.EventSubscription
        {
            Id = source.Id,
            EventId = source.EventId,
            MemberId = source.MemberId,
            IsWaiting = source.IsWaiting,
            WaitingRank = source.WaitingRank,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.EventSubscription DomainToEntity(this Do.EventSubscription source)
    {
        return new En.EventSubscription
        {
            Id = source.Id,
            EventId = source.EventId,
            MemberId = source.MemberId,
            IsWaiting = source.IsWaiting,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.EventSubscription EntityToDomain(this En.EventSubscription source)
    {
        return new Do.EventSubscription
        {
            Id = source.Id,
            EventId = source.EventId,
            MemberId = source.MemberId,
            IsWaiting = source.IsWaiting,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.EventSubscription ContractToDomain(this Ct.EventSubscription source, Guid id)
    {
        return new Do.EventSubscription
        {
            Id = id,
            EventId = source.EventId,
            MemberId = source.MemberId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
