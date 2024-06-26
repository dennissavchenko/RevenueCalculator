namespace Revenue.Entities;

public class Subscription
{
    public int IdSubscription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime NextPaymentDate { get; set; }
    public double Price { get; set; }
    public int RenewalPeriodDays { get; set; }
    public bool IsCancelled { get; set; }
    public int IdSoftware { get; set; }
    public int IdClient { get; set; }
    public virtual Software Software { get; set; }
    public virtual Client Client { get; set; }
    public virtual ICollection<SubscriptionPayment> SubscriptionPayments { get; set; }
}