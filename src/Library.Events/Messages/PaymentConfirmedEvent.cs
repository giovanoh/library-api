namespace Library.Events.Messages;

public class PaymentConfirmedEvent
{
    public int OrderId { get; set; }
    public DateTime ConfirmedAt { get; set; }
}