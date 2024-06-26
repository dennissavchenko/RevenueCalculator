namespace Revenue.Entities;

public class SubscriptionPayment
{
    public int IdSubscriptionPayment { get; set; }
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public int IdSubscription { get; set; }
    public virtual Subscription Subscription { get; set; }
}