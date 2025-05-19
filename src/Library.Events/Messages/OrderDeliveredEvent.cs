namespace Library.Events.Messages;

public class OrderDeliveredEvent
{
    public int OrderId { get; set; }
    public DateTime DeliveredAt { get; set; }
}