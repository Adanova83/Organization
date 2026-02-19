using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ActivitySessionSlot
{
    public static Ct.ActivitySessionSlot DomainToContract(this Do.ActivitySessionSlot source)
    {
        return new Ct.ActivitySessionSlot
        {
            Id = source.Id,
            DayOfWeek = source.DayOfWeek,
            EndTime = source.EndTime,
            SessionId = source.SessionId,
            SpaceId = source.SpaceId,
            StartTime = source.StartTime,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ActivitySessionSlot DomainToEntity(this Do.ActivitySessionSlot source)
    {
        return new En.ActivitySessionSlot
        {
            Id = source.Id,
            DayOfWeek = source.DayOfWeek,
            EndTime = source.EndTime,
            SessionId = source.SessionId,
            SpaceId = source.SpaceId,
            StartTime = source.StartTime,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ActivitySessionSlot EntityToDomain(this En.ActivitySessionSlot source)
    {
        return new Do.ActivitySessionSlot
        {
            Id = source.Id,
            DayOfWeek = source.DayOfWeek,
            EndTime = source.EndTime,
            SessionId = source.SessionId,
            SpaceId = source.SpaceId,
            StartTime = source.StartTime,
            Active = source.Active.EntityToDomain()
        };
    }
}
