using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Activity
{
    public static Ct.Activity DomainToContract(this Do.Activity source)
    {
        return new Ct.Activity
        {
            Id = source.Id,
            Active = source.Active.DomainToContract(),
            AgeMinimum = source.AgeMinimum,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Code = source.Code,
            Description = source.Description,
            Name = source.Name,
            OrganizationId = source.OrganizationId
        };
    }

    internal static En.Activity DomainToEntity(this Do.Activity source)
    {
        return new En.Activity
        {
            Id = source.Id,
            Active = source.Active.DomainToEntity(),
            AgeMinimum = source.AgeMinimum,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Code = source.Code,
            Description = source.Description,
            Name = source.Name,
            OrganizationId = source.OrganizationId
        };
    }

    internal static Do.Activity EntityToDomain(this En.Activity source)
    {
        return new Do.Activity
        {
            Id = source.Id,
            Active = source.Active.EntityToDomain(),
            AgeMinimum = source.AgeMinimum,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            Code = source.Code,
            Description = source.Description,
            Name = source.Name,
            OrganizationId = source.OrganizationId,
            Audit = source.Audit.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.Activity ContractToDomain(this Ct.Activity source, Guid id)
    {
        return new Do.Activity
        {
            Id = id,
            Code = source.Code,
            Name = source.Name,
            Description = source.Description,
            Picture = source.Picture,
            RGBColor = source.RGBColor,
            AgeMinimum = source.AgeMinimum,
            OrganizationId = source.OrganizationId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
