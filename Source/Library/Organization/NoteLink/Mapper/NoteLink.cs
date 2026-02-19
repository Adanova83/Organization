using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class NoteLink
{
    public static Ct.NoteLink DomainToContract(this Do.NoteLink source)
    {
        return new Ct.NoteLink
        {
            Id = source.Id,
            LinkedId = source.LinkedId,
            LinkedType = source.LinkedType,
            NoteId = source.NoteId
        };
    }

    internal static En.NoteLink DomainToEntity(this Do.NoteLink source)
    {
        return new En.NoteLink
        {
            Id = source.Id,
            LinkedId = source.LinkedId,
            LinkedType = source.LinkedType,
            NoteId = source.NoteId
        };
    }

    internal static Do.NoteLink EntityToDomain(this Entity.NoteLink source)
    {
        return new Do.NoteLink
        {
            Id = source.Id,
            LinkedId = source.LinkedId,
            LinkedType = source.LinkedType,
            NoteId = source.NoteId
        };
    }
}
