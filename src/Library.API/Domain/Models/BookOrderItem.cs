namespace Library.API.Domain.Models;

public class BookOrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int BookOrderId { get; set; }
}
