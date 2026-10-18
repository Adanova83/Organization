namespace Library.Repository;

public interface IMemberContact : IRepository<Domain.MemberContact>
{
    /// <summary>Liens filtrés par membre et/ou par contact, avec la fiche du contact.</summary>
    Task<RepositoryResponseRange<Domain.MemberContact>> SearchAsync(Guid? memberId, Guid? contactId, int page, int pageSize, CancellationToken ct);

    /// <summary>Contacts de plusieurs membres à la fois (liste des participants), avec la fiche du contact.</summary>
    Task<IReadOnlyList<Domain.MemberContact>> ByMembersAsync(IReadOnlyCollection<Guid> memberIds, CancellationToken ct);
}
