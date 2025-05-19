namespace Library.Events.Messages;

public class OrderCompletedEvent
{
    public int OrderId { get; set; }
    public DateTime CompletedAt { get; set; }
}