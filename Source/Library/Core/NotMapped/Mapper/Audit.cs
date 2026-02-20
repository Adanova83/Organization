using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Audit
{
    public static Ct.Audit DomainToContract(this Do.Audit source)
    {
        return new Ct.Audit
        {
            CreatedAt = source.CreatedAt,
            DeletedAt = source.DeletedAt,
            LastUpdatedAt = source.LastUpdatedAt
        };
    }

    public static En.Audit DomainToEntity(this Do.Audit source)
    {
        return new En.Audit
        {
            CreatedAt = source.CreatedAt,
            DeletedAt = source.DeletedAt,
            LastUpdatedAt = source.LastUpdatedAt
        };
    }

    public static Do.Audit EntityToDomain(this En.Audit source)
    {
        return new Do.Audit
        {
            CreatedAt = source.CreatedAt,
            DeletedAt = source.DeletedAt,
            LastUpdatedAt = source.LastUpdatedAt
        };
    }
}
