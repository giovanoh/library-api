namespace Library.API.DTOs;

public class AuthorDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Biography { get; set; }
}