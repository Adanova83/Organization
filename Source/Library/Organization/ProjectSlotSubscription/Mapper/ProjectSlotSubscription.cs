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
}
