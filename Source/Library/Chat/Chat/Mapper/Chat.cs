using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Chat
{
    public static Ct.Chat DomainToContract(this Do.Chat source)
    {
        return new Ct.Chat
        {
            Id = source.Id,
            Title = source.Title,
            IsGroup = source.IsGroup,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Chat DomainToEntity(this Do.Chat source)
    {
        return new En.Chat
        {
            Id = source.Id,
            Title = source.Title,
            IsGroup = source.IsGroup,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Chat EntityToDomain(this En.Chat source)
    {
        return new Do.Chat
        {
            Id = source.Id,
            Title = source.Title,
            IsGroup = source.IsGroup,
            Active = source.Active.EntityToDomain()
        };
    }
}
