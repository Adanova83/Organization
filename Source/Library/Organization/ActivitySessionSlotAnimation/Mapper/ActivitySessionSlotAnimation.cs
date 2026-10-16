using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ActivitySessionSlotAnimation
{
    public static Ct.ActivitySessionSlotAnimation DomainToContract(this Do.ActivitySessionSlotAnimation source)
    {
        return new Ct.ActivitySessionSlotAnimation
        {
            Id = source.Id,
            MemberId = source.MemberId,
            Order = source.Order,
            SessionSlotId = source.SessionSlotId,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ActivitySessionSlotAnimation DomainToEntity(this Do.ActivitySessionSlotAnimation source)
    {
        return new En.ActivitySessionSlotAnimation
        {
            Id = source.Id,
            MemberId = source.MemberId,
            Order = source.Order,
            SessionSlotId = source.SessionSlotId,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ActivitySessionSlotAnimation EntityToDomain(this En.ActivitySessionSlotAnimation source)
    {
        return new Do.ActivitySessionSlotAnimation
        {
            Id = source.Id,
            MemberId = source.MemberId,
            Order = source.Order,
            SessionSlotId = source.SessionSlotId,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.ActivitySessionSlotAnimation ContractToDomain(this Ct.ActivitySessionSlotAnimation source, Guid id)
    {
        return new Do.ActivitySessionSlotAnimation
        {
            Id = id,
            SessionSlotId = source.SessionSlotId,
            MemberId = source.MemberId,
            Order = source.Order,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
