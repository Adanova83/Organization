using Library.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository;

public sealed class Participant : IParticipant
{
    private readonly FullDbContext _dbContext;

    public Participant(FullDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private sealed record Subscribed(Guid MemberId, bool IsWaiting, DateTime CreatedAt);

    public async Task<IReadOnlyList<Contract.Participant>> ListAsync(Contract.SubscriptionTargetKind kind, Guid targetId, CancellationToken ct)
    {
        // Inscriptions de l'élément : membre, attente, date d'inscription (ordre de la liste d'attente).
        var subscriptions = kind switch
        {
            Contract.SubscriptionTargetKind.ActivitySlot => await _dbContext.ActivitySessionSlotSubscriptions
                .Where(item => item.SessionSlotId == targetId)
                .Select(item => new Subscribed(item.MemberId, item.IsWaiting, item.Audit.CreatedAt))
                .ToListAsync(ct),
            Contract.SubscriptionTargetKind.Event => await _dbContext.EventSubscriptions
                .Where(item => item.EventId == targetId)
                .Select(item => new Subscribed(item.MemberId, item.IsWaiting, item.Audit.CreatedAt))
                .ToListAsync(ct),
            _ => await _dbContext.ProjectSlotSubscriptions
                .Where(item => item.ProjectSlotId == targetId)
                .Select(item => new Subscribed(item.MemberId, item.IsWaiting, item.Audit.CreatedAt))
                .ToListAsync(ct)
        };

        var memberIds = subscriptions.Select(item => item.MemberId).Distinct().ToList();

        var members = await _dbContext.Members
            .Where(item => memberIds.Contains(item.Id))
            .Select(item => new { item.Id, item.Firstname, item.Lastname, item.Birthdate, item.PhoneNumber, item.Email })
            .ToDictionaryAsync(item => item.Id, ct);

        var contacts = (await _dbContext.MemberContacts
                .Where(item => memberIds.Contains(item.MemberId))
                .Select(item => new
                {
                    item.MemberId,
                    item.Contact.Firstname,
                    item.Contact.Lastname,
                    item.Relation,
                    item.Contact.PhoneNumber,
                    item.Contact.Email,
                    item.IsEmergencyContact,
                    item.CanPickUp
                })
                .ToListAsync(ct))
            .GroupBy(item => item.MemberId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderByDescending(item => item.IsEmergencyContact)
                    .Select(item => new Contract.ParticipantContact
                    {
                        Firstname = item.Firstname,
                        Lastname = item.Lastname,
                        Relation = (Contract.ContactRelation)item.Relation,
                        PhoneNumber = item.PhoneNumber,
                        Email = item.Email,
                        IsEmergencyContact = item.IsEmergencyContact,
                        CanPickUp = item.CanPickUp
                    })
                    .ToList());

        var waitingRanks = subscriptions
            .Where(item => item.IsWaiting)
            .OrderBy(item => item.CreatedAt)
            .Select((item, index) => (item.MemberId, Rank: index + 1))
            .ToDictionary(item => item.MemberId, item => item.Rank);

        return subscriptions
            .Where(item => members.ContainsKey(item.MemberId))
            .Select(item =>
            {
                var member = members[item.MemberId];
                return new Contract.Participant
                {
                    MemberId = member.Id,
                    Firstname = member.Firstname,
                    Lastname = member.Lastname,
                    Birthdate = member.Birthdate,
                    PhoneNumber = member.PhoneNumber,
                    Email = member.Email,
                    IsWaiting = item.IsWaiting,
                    WaitingRank = item.IsWaiting ? waitingRanks.GetValueOrDefault(item.MemberId) : null,
                    Contacts = contacts.GetValueOrDefault(item.MemberId) ?? []
                };
            })
            .OrderBy(item => item.IsWaiting)
            .ThenBy(item => item.WaitingRank)
            .ThenBy(item => item.Lastname)
            .ThenBy(item => item.Firstname)
            .ToList();
    }

    public Task<bool> IsAnimatorAsync(Contract.SubscriptionTargetKind kind, Guid targetId, Guid memberId, CancellationToken ct)
        => kind switch
        {
            Contract.SubscriptionTargetKind.ActivitySlot => _dbContext.ActivitySessionSlotAnimations
                .AnyAsync(item => item.SessionSlotId == targetId && item.MemberId == memberId, ct),
            Contract.SubscriptionTargetKind.Event => _dbContext.EventAnimations
                .AnyAsync(item => item.EventId == targetId && item.MemberId == memberId, ct),
            _ => _dbContext.ProjectSlotAnimations
                .AnyAsync(item => item.ProjectSlotId == targetId && item.MemberId == memberId, ct)
        };
}
