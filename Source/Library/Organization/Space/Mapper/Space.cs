using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Space
{
    public static Ct.Space DomainToContract(this Do.Space source)
    {
        return new Ct.Space
        {
            Id = source.Id,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Space DomainToEntity(this Do.Space source)
    {
        return new En.Space
        {
            Id = source.Id,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Space EntityToDomain(this En.Space source)
    {
        return new Do.Space
        {
            Id = source.Id,
            Name = source.Name,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Active = source.Active.EntityToDomain(),
            Audit = source.Audit.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Space ContractToDomain(this Ct.Space source, Guid id)
    {
        return new Do.Space
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
