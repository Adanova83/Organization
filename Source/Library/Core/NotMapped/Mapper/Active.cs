using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class Active
{
    public static Ct.Active DomainToContract(this Do.Active source)
    {
        return new Ct.Active
        {
            From = source.From,
            Until = source.Until
        };
    }

    public static En.Active DomainToEntity(this Do.Active source)
    {
        return new En.Active
        {
            From = source.From,
            Until = source.Until
        };
    }

    public static Do.Active EntityToDomain(this En.Active source)
    {
        return new Do.Active
        {
            From = source.From,
            Until = source.Until
        };
    }
}
