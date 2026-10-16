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
            Active = source.Active.EntityToDomain(),
            Audit = source.Audit.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.ProjectSlot ContractToDomain(this Ct.ProjectSlot source, Guid id)
    {
        return new Do.ProjectSlot
        {
            Id = id,
            DayOfWeek = source.DayOfWeek,
            StartTime = source.StartTime,
            EndTime = source.EndTime,
            ProjectId = source.ProjectId,
            SpaceId = source.SpaceId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
