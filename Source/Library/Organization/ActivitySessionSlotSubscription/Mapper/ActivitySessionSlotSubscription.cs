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
            SessionSlotId = source.SessionSlotId,
            Active = source.Active.EntityToDomain()
        };
    }
}
