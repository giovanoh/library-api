using System.ComponentModel.DataAnnotations;

namespace Library.API.DTOs;

public record SaveAuthorDto
{
    [Required]
    [MaxLength(100)]
    public string? Name { get; init; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; init; }

    [MaxLength(1000)]
    public string? Biography { get; init; }
}
