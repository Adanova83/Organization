using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ChatMessageRecipient
{
    public static Ct.ChatMessageRecipient DomainToContract(this Do.ChatMessageRecipient source)
    {
        return new Ct.ChatMessageRecipient
        {
            Id = source.Id,
            ChatMessageId = source.ChatMessageId,
            RecipentId = source.RecipentId,
            ReceiveDate = source.ReceiveDate,
            ReadDate = source.ReadDate,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ChatMessageRecipient DomainToEntity(this Do.ChatMessageRecipient source)
    {
        return new En.ChatMessageRecipient
        {
            Id = source.Id,
            ChatMessageId = source.ChatMessageId,
            RecipentId = source.RecipentId,
            ReceiveDate = source.ReceiveDate,
            ReadDate = source.ReadDate,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.ChatMessageRecipient EntityToDomain(this En.ChatMessageRecipient source)
    {
        return new Do.ChatMessageRecipient
        {
            Id = source.Id,
            ChatMessageId = source.ChatMessageId,
            RecipentId = source.RecipentId,
            ReceiveDate = source.ReceiveDate,
            ReadDate = source.ReadDate,
            Active = source.Active.EntityToDomain()
        };
    }
}
