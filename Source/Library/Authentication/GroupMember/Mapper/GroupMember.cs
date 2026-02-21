using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class GroupMember
{
    public static Ct.GroupMember DomainToContract(this Do.GroupMember source)
    {
        return new Ct.GroupMember
        {
            Id = source.Id,
            MemberId = source.MemberId,
            GroupId = source.GroupId,
            Audit = source.Audit.DomainToContract(),
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.GroupMember DomainToEntity(this Do.GroupMember source)
    {
        return new En.GroupMember
        {
            Id = source.Id,
            MemberId = source.MemberId,
            GroupId = source.GroupId,
            Audit = source.Audit.DomainToEntity(),
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.GroupMember EntityToDomain(this En.GroupMember source)
    {
        return new Do.GroupMember
        {
            Id = source.Id,
            MemberId = source.MemberId,
            GroupId = source.GroupId,
            Audit = source.Audit.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }
}
