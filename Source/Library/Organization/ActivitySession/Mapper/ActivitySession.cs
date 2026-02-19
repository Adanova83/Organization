using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ActivitySession
{
    public static Ct.ActivitySession DomainToContract(this Do.ActivitySession source)
    {
        return new Ct.ActivitySession
        {
            Id = source.Id,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            ActivityId = source.ActivityId,
            OrganizationSessionId = source.OrganizationSessionId,
            Price = source.Price,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ActivitySession DomainToEntity(this Do.ActivitySession source)
    {
        return new En.ActivitySession
        {
            Id = source.Id,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            ActivityId = source.ActivityId,
            OrganizationSessionId = source.OrganizationSessionId,
            Price = source.Price,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ActivitySession EntityToDomain(this En.ActivitySession source)
    {
        return new Do.ActivitySession
        {
            Id = source.Id,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            ActivityId = source.ActivityId,
            OrganizationSessionId = source.OrganizationSessionId,
            Price = source.Price,
            Active = source.Active.EntityToDomain()
        };
    }
}
