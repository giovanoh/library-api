namespace Library.Events.Messages;

public class OrderProcessingEvent
{
    public int OrderId { get; set; }
    public DateTime ProcessingAt { get; set; }
}