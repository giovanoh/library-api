using Library.API.Domain.Models;

namespace Library.API.Domain.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> ListAsync();
    Task<Book?> FindByIdAsync(int bookId);
    Task AddAsync(Book book);
    void Update(Book book);
    void Delete(Book book);
}
