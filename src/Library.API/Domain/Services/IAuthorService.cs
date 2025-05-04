using Library.API.Domain.Models;

namespace Library.API.Domain.Services;

public interface IAuthorService
{   
    Task<IEnumerable<Author>> ListAsync();
}
    
