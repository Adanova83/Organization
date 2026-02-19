using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Holiday
{
    public static Ct.Holiday DomainToContract(this Do.Holiday source)
    {
        return new Ct.Holiday
        {
            Id = source.Id,
            EndDate = source.EndDate,
            Name = source.Name,
            StartDate = source.StartDate,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.Holiday DomainToEntity(this Do.Holiday source)
    {
        return new En.Holiday
        {
            Id = source.Id,
            EndDate = source.EndDate,
            Name = source.Name,
            StartDate = source.StartDate,
            Active = source.Active.DomainToEntity()
        };
    }

    internal static Do.Holiday EntityToDomain(this En.Holiday source)
    {
        return new Do.Holiday
        {
            Id = source.Id,
            EndDate = source.EndDate,
            Name = source.Name,
            StartDate = source.StartDate,
            Active = source.Active.EntityToDomain()
        };
    }
}
