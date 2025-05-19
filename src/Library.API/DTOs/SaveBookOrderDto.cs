using System.ComponentModel.DataAnnotations;

namespace Library.API.DTOs;

public class SaveBookOrderDto
{
    public List<SaveBookOrderItemDto> Items { get; set; } = [];
}

public class SaveBookOrderItemDto
{
    [Required]
    public int? BookId { get; set; }

    [Required]
    [Range(1, 50)]
    public int Quantity { get; set; }
}
