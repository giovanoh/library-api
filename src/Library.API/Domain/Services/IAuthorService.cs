using Library.API.Domain.Models;
using Library.API.Domain.Services.Communication;

namespace Library.API.Domain.Services;

public interface IAuthorService
{
    Task<Response<IEnumerable<Author>>> ListAsync();
    Task<Response<Author>> FindByIdAsync(int authorId);
    Task<Response<Author>> AddAsync(Author author);
    Task<Response<Author>> UpdateAsync(int authorId, Author author);
    Task<Response<Author>> DeleteAsync(int authorId);
}
    
