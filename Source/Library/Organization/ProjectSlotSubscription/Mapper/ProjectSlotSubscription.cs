using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ProjectSlotSubscription
{
    public static Ct.ProjectSlotSubscription DomainToContract(this Do.ProjectSlotSubscription source)
    {
        return new Ct.ProjectSlotSubscription
        {
            Id = source.Id,
            MemberId = source.MemberId,
            ProjectSlotId = source.ProjectSlotId,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ProjectSlotSubscription DomainToEntity(this Do.ProjectSlotSubscription source)
    {
        return new En.ProjectSlotSubscription
        {
            Id = source.Id,
            MemberId = source.MemberId,
            ProjectSlotId = source.ProjectSlotId,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ProjectSlotSubscription EntityToDomain(this En.ProjectSlotSubscription source)
    {
        return new Do.ProjectSlotSubscription
        {
            Id = source.Id,
            MemberId = source.MemberId,
            ProjectSlotId = source.ProjectSlotId,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.ProjectSlotSubscription ContractToDomain(this Ct.ProjectSlotSubscription source, Guid id)
    {
        return new Do.ProjectSlotSubscription
        {
            Id = id,
            ProjectSlotId = source.ProjectSlotId,
            MemberId = source.MemberId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
