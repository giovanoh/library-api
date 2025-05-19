namespace Library.API.Domain.Models;

public class BookOrder
{
    public int Id { get; set; }
    public DateTime CheckoutDate { get; set; }
    public List<BookOrderItem> Items { get; set; } = [];
    public BookOrderStatus Status { get; set; } = BookOrderStatus.Placed;
}
