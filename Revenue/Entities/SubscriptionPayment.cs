namespace Revenue.Entities;

public class SubscriptionPayment
{
    public int IdSubscriptionPayment { get; set; }
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public int IdSubscriptionClient { get; set; }
    public virtual SubscriptionClient SubscriptionClient { get; set; }
}