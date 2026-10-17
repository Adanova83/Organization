namespace Library.Contract;

public class ActivitySessionSlotSubscription : PrimaryKey
{
    public Guid SessionSlotId { get; set; }
    public Guid MemberId{ get; set; }

    // Décidés par l'API (ignorés à l'envoi) : sur liste d'attente quand l'élément est complet, et rang dans cette liste.
    public bool IsWaiting { get; set; }
    public int? WaitingRank { get; set; }
}
