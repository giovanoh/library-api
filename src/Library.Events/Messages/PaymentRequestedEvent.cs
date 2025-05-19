namespace Library.Events.Messages;

public class PaymentRequestedEvent
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime RequestedAt { get; set; }
}