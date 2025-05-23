namespace Library.API.Domain.Models;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public string? Biography { get; set; }
    public List<Book> Books { get; set; } = [];
}