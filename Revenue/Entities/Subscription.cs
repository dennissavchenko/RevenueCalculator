using System.ComponentModel.DataAnnotations;
using Revenue.Constant;

namespace Revenue.Entities;

public class Subscription
{
    public int IdSubscription { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    [Range(SubscriptionConstants.MinRenewalPeriodDays, SubscriptionConstants.MaxRenewalPeriodDays)]
    public int RenewalPeriodDays { get; set; }
    public int IdSoftware { get; set; }
    public virtual Software Software { get; set; }
    public virtual ICollection<SubscriptionClient> SubscriptionClients { get; set; }
}