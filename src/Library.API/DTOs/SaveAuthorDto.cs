using System.ComponentModel.DataAnnotations;

using Library.API.Validation;

namespace Library.API.DTOs;

public record SaveAuthorDto
{
    [Required]
    [MaxLength(100)]
    public string? Name { get; init; }

    [Required]
    [PastDate]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; init; }

    [MaxLength(1000)]
    public string? Biography { get; init; }
}
