using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ChatMessage
{
    public static Ct.ChatMessage DomainToContract(this Do.ChatMessage source)
    {
        return new Ct.ChatMessage
        {
            Id = source.Id,
            Title = source.Title,
            Body = source.Body,
            SendDate = source.SendDate,
            SenderId = source.SenderId,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ChatMessage DomainToEntity(this Do.ChatMessage source)
    {
        return new En.ChatMessage
        {
            Id = source.Id,
            Title = source.Title,
            Body = source.Body,
            SendDate = source.SendDate,
            SenderId = source.SenderId,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ChatMessage EntityToDomain(this En.ChatMessage source)
    {
        return new Do.ChatMessage
        {
            Id = source.Id,
            Title = source.Title,
            Body = source.Body,
            SendDate = source.SendDate,
            SenderId = source.SenderId,
            Active = source.Active.EntityToDomain()
        };
    }
}
