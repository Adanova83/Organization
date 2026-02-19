using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Note
{
    public static Ct.Note DomainToContract(this Do.Note source)
    {
        return new Ct.Note
        {
            Id = source.Id,
            Title = source.Title,
            Body = source.Body,
            RGBBackgroundColor = source.RGBBackgroundColor,            
            MemberId = source.MemberId
        };
    }

    internal static En.Note DomainToEntity(this Do.Note source)
    {
        return new En.Note
        {
            Id = source.Id,
            Title = source.Title,
            Body = source.Body,
            RGBBackgroundColor = source.RGBBackgroundColor,
            MemberId = source.MemberId
        };
    }

    internal static Do.Note EntityToDomain(this Entity.Note source)
    {
        return new Do.Note
        {
            Id = source.Id,
            Title = source.Title,
            Body = source.Body,
            RGBBackgroundColor = source.RGBBackgroundColor,
            MemberId = source.MemberId
        };
    }
}
