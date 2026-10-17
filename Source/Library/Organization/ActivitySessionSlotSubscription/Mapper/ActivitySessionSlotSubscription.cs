using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ActivitySessionSlotSubscription
{
    public static Ct.ActivitySessionSlotSubscription DomainToContract(this Do.ActivitySessionSlotSubscription source)
    {
        return new Ct.ActivitySessionSlotSubscription
        {
            Id = source.Id,
            MemberId = source.MemberId,
            IsWaiting = source.IsWaiting,
            WaitingRank = source.WaitingRank,
            SessionSlotId = source.SessionSlotId,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ActivitySessionSlotSubscription DomainToEntity(this Do.ActivitySessionSlotSubscription source)
    {
        return new En.ActivitySessionSlotSubscription
        {
            Id = source.Id,
            MemberId = source.MemberId,
            IsWaiting = source.IsWaiting,
            SessionSlotId = source.SessionSlotId,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ActivitySessionSlotSubscription EntityToDomain(this En.ActivitySessionSlotSubscription source)
    {
        return new Do.ActivitySessionSlotSubscription
        {
            Id = source.Id,
            MemberId = source.MemberId,
            IsWaiting = source.IsWaiting,
            SessionSlotId = source.SessionSlotId,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.ActivitySessionSlotSubscription ContractToDomain(this Ct.ActivitySessionSlotSubscription source, Guid id)
    {
        return new Do.ActivitySessionSlotSubscription
        {
            Id = id,
            SessionSlotId = source.SessionSlotId,
            MemberId = source.MemberId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
