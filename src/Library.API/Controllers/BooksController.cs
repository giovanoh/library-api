using System.Diagnostics;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.DTOs;
using Library.API.DTOs.Response;

namespace Library.API.Controllers;

/// <summary>
/// Controller responsible for managing book-related operations
/// </summary>
public class BooksController : ApiController
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;
    private readonly ActivitySource _activitySource;

    /// <summary>
    /// Initializes a new instance of the BooksController
    /// </summary>
    /// <param name="bookService">Service for handling book operations</param>
    /// <param name="mapper">Object mapping service</param>
    /// <param name="activitySource">Activity source for tracing</param>
    public BooksController(IBookService bookService, IMapper mapper, ActivitySource activitySource)
    {
        _bookService = bookService;
        _mapper = mapper;
        _activitySource = activitySource;
    }

    /// <summary>
    /// Retrieves all registered books
    /// </summary>
    /// <returns>List of books</returns>
    /// <response code="200">Returns the list of books</response>
    /// <response code="400">Validation Error</response>
    /// <response code="500">Internal Server Error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookDto>>), 200)]
    [ProducesResponseType(typeof(ApiProblemDetails), 400)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> GetAllAsync()
    {
        using var activity = _activitySource.StartActivity("Controller: BooksController.GetAllAsync");
        var result = await _bookService.ListAsync();
        if (!result.Success)
            return HandleErrorResponse(result);

        var booksDto = _mapper.Map<IEnumerable<BookDto>>(result.Model);
        return Success(booksDto);
    }

    /// <summary>
    /// Retrieves a specific book by its unique identifier
    /// </summary>
    /// <param name="id">Unique identifier of the book</param>
    /// <returns>Book details</returns>
    /// <response code="200">Returns the book details</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error</response>
    [HttpGet("{id}", Name = "GetBookById")]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), 200)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        using var activity = _activitySource.StartActivity("Controller: BooksController.GetByIdAsync");
        var result = await _bookService.FindByIdAsync(id);
        if (!result.Success)
            return HandleErrorResponse(result);

        var bookDto = _mapper.Map<BookDto>(result.Model);
        return Success(bookDto);
    }

    /// <summary>
    /// Creates a new book
    /// </summary>
    /// <param name="saveBookDto">Data for creating the book</param>
    /// <returns>Created book</returns>
    /// <response code="201">Book created successfully</response>
    /// <response code="400">Validation Error</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), 201)]
    [ProducesResponseType(typeof(ApiProblemDetails), 400)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> CreateAsync([FromBody] SaveBookDto saveBookDto)
    {
        using var activity = _activitySource.StartActivity("Controller: BooksController.CreateAsync");
        var book = _mapper.Map<Book>(saveBookDto);
        var result = await _bookService.AddAsync(book);
        if (!result.Success)
            return HandleErrorResponse(result);

        var bookDto = _mapper.Map<BookDto>(result.Model);
        return Created("GetBookById", new { id = bookDto.Id }, bookDto);
    }

    /// <summary>
    /// Updates an existing book's data
    /// </summary>
    /// <param name="id">Identifier of the book to be updated</param>
    /// <param name="saveBookDto">New book data</param>
    /// <returns>Updated book</returns>
    /// <response code="200">Book updated successfully</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), 200)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SaveBookDto saveBookDto)
    {
        using var activity = _activitySource.StartActivity("Controller: BooksController.UpdateAsync");
        var book = _mapper.Map<Book>(saveBookDto);
        var result = await _bookService.UpdateAsync(id, book);
        if (!result.Success)
            return HandleErrorResponse(result);

        var bookDto = _mapper.Map<BookDto>(result.Model);
        return Success(bookDto);
    }

    /// <summary>
    /// Removes a book by its identifier
    /// </summary>
    /// <param name="id">Identifier of the book to be removed</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Book removed successfully</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        using var activity = _activitySource.StartActivity("Controller: BooksController.DeleteAsync");
        var result = await _bookService.DeleteAsync(id);
        if (!result.Success)
            return HandleErrorResponse(result);

        return NoContent();
    }
}
