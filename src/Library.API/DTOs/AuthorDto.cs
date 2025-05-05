namespace Library.API.DTOs;

public record AuthorDto
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public DateTime? BirthDate { get; init; }
    public string? Biography { get; init; }
}