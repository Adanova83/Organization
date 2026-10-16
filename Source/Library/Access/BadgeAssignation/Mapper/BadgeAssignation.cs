using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class BadgeAssignation
{
    public static Ct.BadgeAssignation DomainToContract(this Do.BadgeAssignation source)
    {
        return new Ct.BadgeAssignation
        {
            Id = source.Id,
            BadgeId = source.BadgeId,
            MemberId = source.MemberId,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.BadgeAssignation DomainToEntity(this Do.BadgeAssignation source)
    {
        return new En.BadgeAssignation
        {
            Id = source.Id,
            BadgeId = source.BadgeId,
            MemberId = source.MemberId,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.BadgeAssignation EntityToDomain(this En.BadgeAssignation source)
    {
        return new Do.BadgeAssignation
        {
            Id = source.Id,
            BadgeId = source.BadgeId,
            MemberId = source.MemberId,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.BadgeAssignation ContractToDomain(this Ct.BadgeAssignation source, Guid id)
    {
        return new Do.BadgeAssignation
        {
            Id = id,
            BadgeId = source.BadgeId,
            MemberId = source.MemberId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
