using Microsoft.EntityFrameworkCore;

using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Domain.Services;
using Library.API.Domain.Services.Communication;

namespace Library.API.Infrastructure.Services;

public class AuthorService : BaseService, IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly System.Diagnostics.ActivitySource _activitySource;

    public AuthorService(IAuthorRepository authorRepository, IUnitOfWork unitOfWork, ILogger<AuthorService> logger, System.Diagnostics.ActivitySource activitySource)
        : base(unitOfWork, logger)
    {
        _authorRepository = authorRepository;
        _activitySource = activitySource;
    }

    public async Task<Response<IEnumerable<Author>>> ListAsync()
    {
        using var activity = _activitySource.StartActivity("Service: AuthorService.ListAsync");
        try
        {
            return Response<IEnumerable<Author>>.Ok(await _authorRepository.ListAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while listing authors.");
            return Response<IEnumerable<Author>>.Fail(
                "An error occurred while retrieving the authors",
                ErrorType.DatabaseError);
        }
    }

    public async Task<Response<Author>> FindByIdAsync(int authorId)
    {
        using var activity = _activitySource.StartActivity("Service: AuthorService.FindByIdAsync");
        try
        {
            var author = await _authorRepository.FindByIdAsync(authorId);
            if (author == null)
                return Response<Author>.NotFound($"Author with id {authorId} was not found");

            return Response<Author>.Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while finding author {AuthorId}.", authorId);
            return Response<Author>.Fail(
                "An error occurred while retrieving the author",
                ErrorType.DatabaseError);
        }
    }

    public async Task<Response<Author>> AddAsync(Author author)
    {
        using var activity = _activitySource.StartActivity("Service: AuthorService.AddAsync");
        try
        {
            await _authorRepository.AddAsync(author);
            await SaveChangesAsync();

            return Response<Author>.Ok(author);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error occurred while saving author to database.");
            return Response<Author>.Fail(
                "An error occurred while saving the author",
                ErrorType.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while adding author.");
            return Response<Author>.Fail(
                "An unexpected error occurred while processing your request",
                ErrorType.Unknown);
        }
    }

    public async Task<Response<Author>> UpdateAsync(int authorId, Author author)
    {
        using var activity = _activitySource.StartActivity("Service: AuthorService.UpdateAsync");
        try
        {
            var existingAuthor = await _authorRepository.FindByIdAsync(authorId);
            if (existingAuthor == null)
                return Response<Author>.NotFound($"Author with id {authorId} was not found");

            UpdateAuthorProperties(existingAuthor, author);

            _authorRepository.Update(existingAuthor);
            await SaveChangesAsync();

            return Response<Author>.Ok(existingAuthor);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error occurred while updating author {AuthorId}.", authorId);
            return Response<Author>.Fail(
                "An error occurred while updating the author",
                ErrorType.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating author {AuthorId}.", authorId);
            return Response<Author>.Fail(
                "An unexpected error occurred while processing your request",
                ErrorType.Unknown);
        }
    }

    public async Task<Response<Author>> DeleteAsync(int authorId)
    {
        using var activity = _activitySource.StartActivity("Service: AuthorService.DeleteAsync");
        try
        {
            var existingAuthor = await _authorRepository.FindByIdAsync(authorId);
            if (existingAuthor == null)
                return Response<Author>.NotFound($"Author with id {authorId} was not found");

            _authorRepository.Delete(existingAuthor);
            await SaveChangesAsync();

            return Response<Author>.Ok(existingAuthor);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error occurred while deleting author {AuthorId}.", authorId);
            return Response<Author>.Fail(
                "An error occurred while deleting the author",
                ErrorType.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while deleting author {AuthorId}.", authorId);
            return Response<Author>.Fail(
                "An unexpected error occurred while processing your request",
                ErrorType.Unknown);
        }
    }

    private static void UpdateAuthorProperties(Author existingAuthor, Author newAuthor)
    {
        existingAuthor.Name = newAuthor.Name;
        existingAuthor.BirthDate = newAuthor.BirthDate;
        existingAuthor.Biography = newAuthor.Biography;
    }
}