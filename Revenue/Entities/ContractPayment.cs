namespace Revenue.Entities;

public class ContractPayment
{
    public int IdContractPayment { get; set; }
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public int IdContract { get; set; }
    public int IdPaymentStatus { get; set; }
    public virtual Contract Contract { get; set; }
    public virtual PaymentStatus PaymentStatus { get; set; }
}