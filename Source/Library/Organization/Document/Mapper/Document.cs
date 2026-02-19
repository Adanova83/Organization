using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Document
{
    public static Ct.Document DomainToContract(this Do.Document source)
    {
        return new Ct.Document
        {
            Id = source.Id,
            Title = source.Title,
            MemberId = source.MemberId,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Document DomainToEntity(this Do.Document source)
    {
        return new En.Document
        {
            Id = source.Id,
            Title = source.Title,
            MemberId = source.MemberId,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Document EntityToDomain(this En.Document source)
    {
        return new Do.Document
        {
            Id = source.Id,
            Title = source.Title,
            MemberId = source.MemberId,
            Active = source.Active.EntityToDomain()
        };
    }
}
