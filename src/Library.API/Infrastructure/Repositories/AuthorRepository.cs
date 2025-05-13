using Microsoft.EntityFrameworkCore;

using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Infrastructure.Contexts;

namespace Library.API.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApiDbContext _context;
    private readonly System.Diagnostics.ActivitySource _activitySource;

    public AuthorRepository(ApiDbContext context, System.Diagnostics.ActivitySource activitySource)
    {
        _context = context;
        _activitySource = activitySource;
    }

    public async Task<IEnumerable<Author>> ListAsync()
    {
        using var activity = _activitySource.StartActivity("Repository: AuthorRepository.ListAsync");
        return await _context.Authors.AsNoTracking().ToListAsync();
    }

    public async Task<Author?> FindByIdAsync(int authorId)
    {
        using var activity = _activitySource.StartActivity("Repository: AuthorRepository.FindByIdAsync");
        return await _context.Authors.FindAsync(authorId);
    }

    public async Task AddAsync(Author author)
    {
        using var activity = _activitySource.StartActivity("Repository: AuthorRepository.AddAsync");
        await _context.Authors.AddAsync(author);
    }

    public void Update(Author author)
    {
        using var activity = _activitySource.StartActivity("Repository: AuthorRepository.Update");
        _context.Authors.Update(author);
    }

    public void Delete(Author author)
    {
        using var activity = _activitySource.StartActivity("Repository: AuthorRepository.Delete");
        _context.Authors.Remove(author);
    }
}
