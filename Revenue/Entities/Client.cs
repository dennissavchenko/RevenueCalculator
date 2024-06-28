namespace Revenue.Entities;

public abstract class Client
{
    public int IdClient { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int ClientTypeId { get; set; }
    public virtual ClientType ClientType { get; set; }
    public virtual ICollection<Contract> Contracts { get; set; }
    public virtual ICollection<SubscriptionClient> SubscriptionClients { get; set; }
}