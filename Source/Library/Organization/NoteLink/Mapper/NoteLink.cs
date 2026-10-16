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

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.NoteLink ContractToDomain(this Ct.NoteLink source, Guid id)
    {
        return new Do.NoteLink
        {
            Id = id,
            NoteId = source.NoteId,
            LinkedType = source.LinkedType,
            LinkedId = source.LinkedId,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
