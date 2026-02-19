using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Application
{
    public static Ct.Application DomainToContract(this Do.Application source)
    {
        return new Ct.Application
        {
            Id = source.Id,
            Name = source.Name,
            Location = source.Location,
            ApiKey = source.ApiKey,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Application DomainToEntity(this Do.Application source)
    {
        return new En.Application
        {
            Id = source.Id,
            Name = source.Name,
            Location = source.Location,
            ApiKey = source.ApiKey,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Application EntityToDomain(this En.Application source)
    {
        return new Do.Application
        {
            Id = source.Id,
            Name = source.Name,
            Location = source.Location,
            ApiKey = source.ApiKey,
            Active = source.Active.EntityToDomain()
        };
    }
}
