
using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Infrastructure.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Library.API.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApiDbContext _context;

    public AuthorRepository(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Author>> ListAsync() {
        return await _context.Authors.AsTracking().ToListAsync();
    }

    public async Task<Author?> FindByIdAsync(int authorId) {
        return await _context.Authors.FindAsync(authorId);
    }

    public async Task AddAsync(Author author) {
        await _context.Authors.AddAsync(author);
    }

    public void UpdateAsync(Author author) {
        _context.Authors.Update(author);
    }

    public void DeleteAsync(Author author) {
        _context.Authors.Remove(author);
    }
}
