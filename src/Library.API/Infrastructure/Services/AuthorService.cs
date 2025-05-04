using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Domain.Services;

namespace Library.API.Infrastructure.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthorService(IAuthorRepository authorRepository, IUnitOfWork unitOfWork)
    {
        _authorRepository = authorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Author>> ListAsync() {
        return await _authorRepository.ListAsync();
    }
}
