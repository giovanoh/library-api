namespace Library.Events.Messages;

public class PaymentFailedEvent
{
    public int OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
}