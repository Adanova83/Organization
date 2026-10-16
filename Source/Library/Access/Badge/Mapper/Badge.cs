using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Badge
{
    public static Ct.Badge DomainToContract(this Do.Badge source)
    {
        return new Ct.Badge
        {
            Id = source.Id,
            Identifier = source.Identifier,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Badge DomainToEntity(this Do.Badge source)
    {
        return new En.Badge
        {
            Id = source.Id,
            Identifier = source.Identifier,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Badge EntityToDomain(this En.Badge source)
    {
        return new Do.Badge
        {
            Id = source.Id,
            Identifier = source.Identifier,
            Active = source.Active.EntityToDomain(),
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Badge ContractToDomain(this Ct.Badge source, Guid id)
    {
        return new Do.Badge
        {
            Id = id,
            Identifier = source.Identifier,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
