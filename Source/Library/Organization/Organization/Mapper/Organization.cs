using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Organization
{
    public static Ct.Organization DomainToContract(this Do.Organization source)
    {
        return new Ct.Organization
        {
            Id = source.Id,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Organization DomainToEntity(this Do.Organization source)
    {
        return new En.Organization
        {
            Id = source.Id,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Organization EntityToDomain(this En.Organization source)
    {
        return new Do.Organization
        {
            Id = source.Id,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Active = source.Active.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Organization ContractToDomain(this Ct.Organization source, Guid id)
    {
        return new Do.Organization
        {
            Id = id,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
