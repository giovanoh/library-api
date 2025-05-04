using System.ComponentModel.DataAnnotations;

namespace Library.API.DTOs;

public class SaveAuthorDto
{
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [MaxLength(1000)]
    public string? Biography { get; set; }
}
