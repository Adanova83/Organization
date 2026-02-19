using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ChatMember
{
    public static Ct.ChatMember DomainToContract(this Do.ChatMember source)
    {
        return new Ct.ChatMember
        {
            Id = source.Id,
            IsAdmin = source.IsAdmin,
            ChatId = source.ChatId,
            MemberId = source.MemberId,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ChatMember DomainToEntity(this Do.ChatMember source)
    {
        return new En.ChatMember
        {
            Id = source.Id,
            IsAdmin = source.IsAdmin,
            ChatId = source.ChatId,
            MemberId = source.MemberId,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ChatMember EntityToDomain(this En.ChatMember source)
    {
        return new Do.ChatMember
        {
            Id = source.Id,
            IsAdmin = source.IsAdmin,
            ChatId = source.ChatId,
            MemberId = source.MemberId,
            Active = source.Active.EntityToDomain()
        };
    }
}
