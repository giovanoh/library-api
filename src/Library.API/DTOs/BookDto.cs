namespace Library.API.DTOs;

public record BookDto
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime? ReleaseDate { get; init; }
    public int AuthorId { get; init; }
    public string? AuthorName { get; init; }
}