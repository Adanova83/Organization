using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class OrganizationSession
{
    public static Ct.OrganizationSession DomainToContract(this Do.OrganizationSession source)
    {
        return new Ct.OrganizationSession
        {
            Id = source.Id,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            OrganizationId = source.OrganizationId,
            Price = source.Price,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.OrganizationSession DomainToEntity(this Do.OrganizationSession source)
    {
        return new En.OrganizationSession
        {
            Id = source.Id,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            OrganizationId = source.OrganizationId,
            Price = source.Price,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.OrganizationSession EntityToDomain(this En.OrganizationSession source)
    {
        return new Do.OrganizationSession
        {
            Id = source.Id,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            OrganizationId = source.OrganizationId,
            Price = source.Price,
            Active = source.Active.EntityToDomain()
        };
    }
}
