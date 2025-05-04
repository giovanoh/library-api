using Library.API.Domain.Models;

namespace Library.API.Domain.Repositories;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> ListAsync();
}
