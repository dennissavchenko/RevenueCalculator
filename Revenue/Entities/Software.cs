namespace Revenue.Entities;

public class Software
{
    public int IdSoftware { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CurrentVersion { get; set; }
    public double SubscriptionCost { get; set; }
    public double UpfrontCost { get; set; }
    public int IdSoftwareCategory { get; set; }
    public virtual SoftwareCategory SoftwareCategory { get; set; }
    public virtual ICollection<Discount> Discounts { get; set; }
    public virtual ICollection<Contract> Contracts { get; set; }
}