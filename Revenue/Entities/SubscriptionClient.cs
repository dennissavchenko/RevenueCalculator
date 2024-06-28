namespace Revenue.Entities;

public class SubscriptionClient
{
    public int IdSubscriptionClient { get; set; }
    public int IdSubscription { get; set; }
    public int IdClient { get; set; }
    public double Price { get; set; }
    public DateTime NextPaymentDate { get; set; }
    public bool IsCancelled { get; set; }
    public virtual Client Client { get; set; }
    public virtual Subscription Subscription { get; set; }
    public virtual ICollection<SubscriptionPayment> SubscriptionPayments { get; set; }
}