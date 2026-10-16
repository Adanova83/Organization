using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Country
{
    public static Ct.Country DomainToContract(this Do.Country source)
    {
        return new Ct.Country
        {
            Id = source.Id,
            CodeAlpha2 = source.CodeAlpha2,
            CodeAlpha3 = source.CodeAlpha3,
            Name = source.Name,
            Audit = source.Audit.DomainToContract(),
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Country DomainToEntity(this Do.Country source)
    {
        return new En.Country
        {
            Id = source.Id,
            CodeAlpha2 = source.CodeAlpha2,
            CodeAlpha3 = source.CodeAlpha3,
            Name = source.Name,
            Audit = source.Audit.DomainToEntity(),
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Country EntityToDomain(this En.Country source)
    {
        return new Do.Country
        {
            Id = source.Id,
            CodeAlpha2 = source.CodeAlpha2,
            CodeAlpha3 = source.CodeAlpha3,
            Name = source.Name,
            Audit = source.Audit.EntityToDomain(),
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Country ContractToDomain(this Ct.Country source, Guid id)
    {
        return new Do.Country
        {
            Id = id,
            CodeAlpha2 = source.CodeAlpha2,
            CodeAlpha3 = source.CodeAlpha3,
            Name = source.Name,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
