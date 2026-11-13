using Library.Repository;

namespace Library.Handler;

/// <summary>Participants d'un élément, pour ses animateurs et les responsables.</summary>
public sealed class Participant
{
    private readonly IParticipant _repository;

    public Participant(IParticipant repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<Contract.Participant>> ListAsync(Contract.SubscriptionTargetKind kind, Guid targetId, CancellationToken ct)
    {
        if (targetId == Guid.Empty)
        {
            throw new ArgumentException("Target ID must not be empty.", nameof(targetId));
        }

        return _repository.ListAsync(kind, targetId, ct);
    }

    /// <summary>Vrai si le membre anime cet élément (créneau, événement ou créneau de projet).</summary>
    public Task<bool> IsAnimatorAsync(Contract.SubscriptionTargetKind kind, Guid targetId, Guid memberId, CancellationToken ct)
        => _repository.IsAnimatorAsync(kind, targetId, memberId, ct);
}
