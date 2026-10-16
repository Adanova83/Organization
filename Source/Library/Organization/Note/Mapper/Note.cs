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

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Note ContractToDomain(this Ct.Note source, Guid id)
    {
        return new Do.Note
        {
            Id = id,
            Title = source.Title,
            Body = source.Body,
            RGBBackgroundColor = source.RGBBackgroundColor,
            MemberId = source.MemberId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
