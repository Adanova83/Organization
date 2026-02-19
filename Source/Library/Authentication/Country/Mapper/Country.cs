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
            Active = source.Active.EntityToDomain()
        };
    }
}
