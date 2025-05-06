
using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Infrastructure.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Library.API.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly ApiDbContext _context;

    public BookRepository(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> ListAsync()
    {
        return await _context.Books.AsNoTracking().Include(p => p.Author).ToListAsync();
    }

    public async Task<Book?> FindByIdAsync(int bookId)
    {
        return await _context.Books.Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == bookId);
    }

    public async Task AddAsync(Book book)
    {
        await _context.Books.AddAsync(book);
    }

    public void Delete(Book book)
    {
        _context.Books.Remove(book);
    }

    public void Update(Book book)
    {
        _context.Books.Update(book);
    }
}