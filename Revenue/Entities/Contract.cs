namespace Revenue.Entities;

public class Contract
{
    public int IdContract { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Price { get; set; }
    public int AdditionalYearsOfSupport { get; set; } 
    public bool IsSigned { get; set; }
    public bool IsCancelled { get; set; }
    public string SoftwareVersion { get; set; }
    public int IdSoftware { get; set; }
    public int IdClient { get; set; }
    public virtual Software Software { get; set; }
    public virtual Client Client { get; set; }
    public virtual ICollection<ContractPayment> ContractPayments { get; set; }
 }