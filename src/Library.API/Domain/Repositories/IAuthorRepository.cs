using Library.API.Domain.Models;

namespace Library.API.Domain.Repositories;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> ListAsync();
    Task<Author?> FindByIdAsync(int authorId);
    Task AddAsync(Author author);
    void Update(Author author);
    void Delete(Author author);
}
