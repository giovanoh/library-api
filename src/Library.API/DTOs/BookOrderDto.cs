namespace Library.API.DTOs;

public class BookOrderDto
{
    public int Id { get; set; }
    public DateTime CheckoutDate { get; set; }
    public List<BookOrderItemDto> Items { get; set; } = [];
    public string Status { get; set; } = string.Empty;
}

public class BookOrderItemDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
