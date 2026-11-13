namespace Library.Repository;

/// <summary>Participants d'un élément (inscrits et liste d'attente) et animateurs de cet élément.</summary>
public interface IParticipant
{
    Task<IReadOnlyList<Contract.Participant>> ListAsync(Contract.SubscriptionTargetKind kind, Guid targetId, CancellationToken ct);

    Task<bool> IsAnimatorAsync(Contract.SubscriptionTargetKind kind, Guid targetId, Guid memberId, CancellationToken ct);
}
