using Microsoft.EntityFrameworkCore;

using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Domain.Services;
using Library.API.Domain.Services.Communication;

namespace Library.API.Infrastructure.Services;

public class BookService : BaseService, IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly System.Diagnostics.ActivitySource _activitySource;
    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, IUnitOfWork unitOfWork, ILogger<BookService> logger, System.Diagnostics.ActivitySource activitySource)
        : base(unitOfWork, logger)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _activitySource = activitySource;
    }

    public async Task<Response<IEnumerable<Book>>> ListAsync()
    {
        using var activity = _activitySource.StartActivity("Service: BookService.ListAsync");
        try
        {
            var books = await _bookRepository.ListAsync();
            return Response<IEnumerable<Book>>.Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while listing books.");
            return Response<IEnumerable<Book>>.Fail(
                "An error occurred while retrieving the books",
                ErrorType.DatabaseError);
        }
    }

    public async Task<Response<Book>> FindByIdAsync(int bookId)
    {
        using var activity = _activitySource.StartActivity("Service: BookService.FindByIdAsync");
        try
        {
            var book = await _bookRepository.FindByIdAsync(bookId);
            if (book == null)
                return Response<Book>.NotFound($"Book with id {bookId} was not found");

            return Response<Book>.Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while finding book {BookId}.", bookId);
            return Response<Book>.Fail(
                "An error occurred while retrieving the book",
                ErrorType.DatabaseError);
        }
    }

    public async Task<Response<Book>> AddAsync(Book book)
    {
        using var activity = _activitySource.StartActivity("Service: BookService.AddAsync");
        try
        {
            var author = await _authorRepository.FindByIdAsync(book.AuthorId);
            if (author == null)
                return Response<Book>.NotFound($"Author with id {book.AuthorId} was not found");

            await _bookRepository.AddAsync(book);
            await SaveChangesAsync();

            return Response<Book>.Ok(book);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error occurred while saving book.");
            return Response<Book>.Fail(
                "An error occurred while saving the book",
                ErrorType.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while adding book.");
            return Response<Book>.Fail(
                "An unexpected error occurred while processing your request",
                ErrorType.Unknown);
        }
    }

    public async Task<Response<Book>> UpdateAsync(int bookId, Book book)
    {
        using var activity = _activitySource.StartActivity("Service: BookService.UpdateAsync");
        try
        {
            var existingBook = await _bookRepository.FindByIdAsync(bookId);
            if (existingBook == null)
                return Response<Book>.NotFound($"Book with id {bookId} was not found");

            var newAuthor = await _authorRepository.FindByIdAsync(book.AuthorId);
            if (newAuthor == null)
                return Response<Book>.NotFound($"Author with id {book.AuthorId} was not found");

            UpdateBookProperties(existingBook, book);

            _bookRepository.Update(existingBook);
            await SaveChangesAsync();

            return Response<Book>.Ok(existingBook);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error occurred while updating book {BookId}.", bookId);
            return Response<Book>.Fail(
                "An error occurred while updating the book",
                ErrorType.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating book {BookId}.", bookId);
            return Response<Book>.Fail(
                "An unexpected error occurred while processing your request",
                ErrorType.Unknown);
        }
    }

    public async Task<Response<Book>> DeleteAsync(int bookId)
    {
        using var activity = _activitySource.StartActivity("Service: BookService.DeleteAsync");
        try
        {
            var book = await _bookRepository.FindByIdAsync(bookId);
            if (book == null)
                return Response<Book>.NotFound($"Book with id {bookId} was not found");

            _bookRepository.Delete(book);
            await SaveChangesAsync();
            return Response<Book>.Ok(book);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error occurred while deleting book {BookId}.", bookId);
            return Response<Book>.Fail(
                "An error occurred while deleting the book",
                ErrorType.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while deleting book {BookId}.", bookId);
            return Response<Book>.Fail(
                "An unexpected error occurred while processing your request",
                ErrorType.Unknown);
        }
    }

    private static void UpdateBookProperties(Book existingBook, Book newBook)
    {
        existingBook.Title = newBook.Title;
        existingBook.Description = newBook.Description;
        existingBook.ReleaseDate = newBook.ReleaseDate;
        existingBook.AuthorId = newBook.AuthorId;
    }
}