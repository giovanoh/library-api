using Library.API.Domain.Models;
using Library.API.Domain.Services.Communication;

namespace Library.API.Domain.Services;

public interface IBookService
{
    Task<Response<IEnumerable<Book>>> ListAsync();
    Task<Response<Book>> FindByIdAsync(int bookId);
    Task<Response<Book>> AddAsync(Book book);
    Task<Response<Book>> UpdateAsync(int bookId, Book book);
    Task<Response<Book>> DeleteAsync(int bookId);
}
