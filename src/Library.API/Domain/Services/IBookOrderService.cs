using Library.API.Domain.Models;
using Library.API.Domain.Services.Communication;

namespace Library.API.Domain.Services;

public interface IBookOrderService
{
    Task<Response<BookOrder>> FindByIdAsync(int id);
    Task<Response<BookOrder>> AddAsync(BookOrder bookOrder);
    Task<Response<BookOrder>> UpdateStatusAsync(int orderId, BookOrderStatus status);
}
