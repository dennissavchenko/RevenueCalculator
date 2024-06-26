namespace Revenue.Entities;

public class PaymentStatus
{
    public int IdPaymentStatus { get; set; }
    public string Name { get; set; }
    public virtual ICollection<ContractPayment> ContractPayments { get; set; }
}