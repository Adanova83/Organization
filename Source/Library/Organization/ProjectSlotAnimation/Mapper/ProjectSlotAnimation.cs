using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ProjectSlotAnimation
{
    public static Ct.ProjectSlotAnimation DomainToContract(this Do.ProjectSlotAnimation source)
    {
        return new Ct.ProjectSlotAnimation
        {
            Id = source.Id,
            MemberId = source.MemberId,
            ProjectSlotId = source.ProjectSlotId,
            Order = source.Order,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ProjectSlotAnimation DomainToEntity(this Do.ProjectSlotAnimation source)
    {
        return new En.ProjectSlotAnimation
        {
            Id = source.Id,
            MemberId = source.MemberId,
            ProjectSlotId = source.ProjectSlotId,
            Order = source.Order,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ProjectSlotAnimation EntityToDomain(this En.ProjectSlotAnimation source)
    {
        return new Do.ProjectSlotAnimation
        {
            Id = source.Id,
            MemberId = source.MemberId,
            ProjectSlotId = source.ProjectSlotId,
            Order = source.Order,
            Active = source.Active.EntityToDomain()
        };
    }
}
