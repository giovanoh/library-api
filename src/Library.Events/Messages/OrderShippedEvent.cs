namespace Library.Events.Messages;

public class OrderShippedEvent
{
    public int OrderId { get; set; }
    public DateTime ShippedAt { get; set; }
}