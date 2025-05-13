using Microsoft.EntityFrameworkCore;

using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Infrastructure.Contexts;

namespace Library.API.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly ApiDbContext _context;
    private readonly System.Diagnostics.ActivitySource _activitySource;

    public BookRepository(ApiDbContext context, System.Diagnostics.ActivitySource activitySource)
    {
        _context = context;
        _activitySource = activitySource;
    }

    public async Task<IEnumerable<Book>> ListAsync()
    {
        using var activity = _activitySource.StartActivity("Repository: BookRepository.ListAsync");
        return await _context.Books.AsNoTracking().Include(p => p.Author).ToListAsync();
    }

    public async Task<Book?> FindByIdAsync(int bookId)
    {
        using var activity = _activitySource.StartActivity("Repository: BookRepository.FindByIdAsync");
        return await _context.Books.Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == bookId);
    }

    public async Task AddAsync(Book book)
    {
        using var activity = _activitySource.StartActivity("Repository: BookRepository.AddAsync");
        await _context.Books.AddAsync(book);
    }

    public void Delete(Book book)
    {
        using var activity = _activitySource.StartActivity("Repository: BookRepository.Delete");
        _context.Books.Remove(book);
    }

    public void Update(Book book)
    {
        using var activity = _activitySource.StartActivity("Repository: BookRepository.Update");
        _context.Books.Update(book);
    }
}