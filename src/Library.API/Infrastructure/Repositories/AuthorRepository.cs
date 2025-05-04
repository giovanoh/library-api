
using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Infrastructure.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Library.API.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApiDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public AuthorRepository(ApiDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Author>> ListAsync() {
        return await _context.Authors.ToListAsync();
    }
}
