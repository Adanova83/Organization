using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Address
{
    public static Ct.Address DomainToContract(this Do.Address source)
    {
        return new Ct.Address
        {
            Line1 = source.Line1,
            Line2 = source.Line2,
            Line3 = source.Line3,
            City = source.City,
            PostalCode = source.PostalCode,
            CountryId = source.CountryId
        };
    }

    internal static En.Address DomainToEntity(this Do.Address source)
    {
        return new En.Address
        {
            Line1 = source.Line1,
            Line2 = source.Line2,
            Line3 = source.Line3,
            City = source.City,
            PostalCode = source.PostalCode,
            CountryId = source.CountryId
        };
    }

    internal static Do.Address EntityToDomain(this En.Address source)
    {
        return new Do.Address
        {
            Line1 = source.Line1,
            Line2 = source.Line2,
            Line3 = source.Line3,
            City = source.City,
            PostalCode = source.PostalCode,
            CountryId = source.CountryId
        };
    }
}
