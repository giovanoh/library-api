namespace Library.Events.Messages;

public class OrderPlacedEvent
{
    public int OrderId { get; set; }
    public DateTime CheckoutDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderPlacedEventItem> Items { get; set; } = new();
}

public class OrderPlacedEventItem
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
}