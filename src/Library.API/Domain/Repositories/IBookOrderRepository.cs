using Library.API.Domain.Models;

namespace Library.API.Infrastructure.Services;

public interface IBookOrderRepository
{
    Task<BookOrder?> FindByIdAsync(int id);
    Task AddAsync(BookOrder bookOrder);
}
