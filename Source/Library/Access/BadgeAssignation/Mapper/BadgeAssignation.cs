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
}
