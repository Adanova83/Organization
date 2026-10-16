using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class EventAnimation
{
    public static Ct.EventAnimation DomainToContract(this Do.EventAnimation source)
    {
        return new Ct.EventAnimation
        {
            Id = source.Id,
            EventId = source.EventId,
            MemberId = source.MemberId,
            Order = source.Order,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.EventAnimation DomainToEntity(this Do.EventAnimation source)
    {
        return new En.EventAnimation
        {
            Id = source.Id,
            EventId = source.EventId,
            MemberId = source.MemberId,
            Order = source.Order,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.EventAnimation EntityToDomain(this En.EventAnimation source)
    {
        return new Do.EventAnimation
        {
            Id = source.Id,
            EventId = source.EventId,
            MemberId = source.MemberId,
            Order = source.Order,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.EventAnimation ContractToDomain(this Ct.EventAnimation source, Guid id)
    {
        return new Do.EventAnimation
        {
            Id = id,
            EventId = source.EventId,
            MemberId = source.MemberId,
            Order = source.Order,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
