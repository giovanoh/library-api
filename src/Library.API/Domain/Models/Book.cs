namespace Library.API.Domain.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? ReleaseDate { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
}