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
            Frequency = source.Frequency,
            SessionId = source.SessionId,
            SpaceId = source.SpaceId,
            Capacity = source.Capacity,
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
            Frequency = source.Frequency,
            SessionId = source.SessionId,
            SpaceId = source.SpaceId,
            Capacity = source.Capacity,
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
            Frequency = source.Frequency,
            SessionId = source.SessionId,
            SpaceId = source.SpaceId,
            Capacity = source.Capacity,
            StartTime = source.StartTime,
            Active = source.Active.EntityToDomain(),
            Audit = source.Audit.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.ActivitySessionSlot ContractToDomain(this Ct.ActivitySessionSlot source, Guid id)
    {
        return new Do.ActivitySessionSlot
        {
            Id = id,
            DayOfWeek = source.DayOfWeek,
            StartTime = source.StartTime,
            EndTime = source.EndTime,
            Frequency = source.Frequency,
            SessionId = source.SessionId,
            SpaceId = source.SpaceId,
            Capacity = source.Capacity,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
