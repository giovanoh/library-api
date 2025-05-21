using System.Diagnostics;

using Microsoft.EntityFrameworkCore;

using Library.API.Domain.Models;
using Library.API.Infrastructure.Contexts;
using Library.API.Infrastructure.Services;

namespace Library.API.Infrastructure.Repositories;

public class BookOrderRepository : IBookOrderRepository
{
    private readonly ApiDbContext _context;
    private readonly ActivitySource _activitySource;

    public BookOrderRepository(ApiDbContext context, ActivitySource activitySource)
    {
        _context = context;
        _activitySource = activitySource;
    }

    public async Task AddAsync(BookOrder bookOrder)
    {
        using var activity = _activitySource.StartActivity("Repository: BookOrderRepository.AddAsync");
        await _context.BookOrders.AddAsync(bookOrder);
    }

    public async Task<BookOrder?> FindByIdAsync(int id)
    {
        using var activity = _activitySource.StartActivity("Repository: BookOrderRepository.FindByIdAsync");
        return await _context.BookOrders
            .Include(o => o.Items)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
