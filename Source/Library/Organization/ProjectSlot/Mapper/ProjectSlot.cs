using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ProjectSlot
{
    public static Ct.ProjectSlot DomainToContract(this Do.ProjectSlot source)
    {
        return new Ct.ProjectSlot
        {
            Id = source.Id,
            DayOfWeek = source.DayOfWeek,
            EndTime = source.EndTime,
            ProjectId = source.ProjectId,
            SpaceId = source.SpaceId,
            StartTime = source.StartTime,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ProjectSlot DomainToEntity(this Do.ProjectSlot source)
    {
        return new En.ProjectSlot
        {
            Id = source.Id,
            DayOfWeek = source.DayOfWeek,
            EndTime = source.EndTime,
            ProjectId = source.ProjectId,
            SpaceId = source.SpaceId,
            StartTime = source.StartTime,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ProjectSlot EntityToDomain(this En.ProjectSlot source)
    {
        return new Do.ProjectSlot
        {
            Id = source.Id,
            DayOfWeek = source.DayOfWeek,
            EndTime = source.EndTime,
            ProjectId = source.ProjectId,
            SpaceId = source.SpaceId,
            StartTime = source.StartTime,
            Active = source.Active.EntityToDomain()
        };
    }
}
