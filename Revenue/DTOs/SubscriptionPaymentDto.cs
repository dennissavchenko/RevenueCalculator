namespace Revenue.DTOs;

public class SubscriptionPaymentDto
{
    public double Amount { get; set; }
    public DateTime Date { get; set; }
    public int IdClient { get; set; }
    public int IdSubscription { get; set; }
}