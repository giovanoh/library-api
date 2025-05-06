using System.ComponentModel.DataAnnotations;

namespace Library.API.DTOs;

public record SaveBookDto
{
    [Required]
    [MaxLength(100)]
    public string? Title { get; init; }

    [MaxLength(1000)]
    public string? Description { get; init; }

    [DataType(DataType.Date)]
    public DateTime? ReleaseDate { get; init; }

    [Required]
    public int AuthorId { get; init; }
}
