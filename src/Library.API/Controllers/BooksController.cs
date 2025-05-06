using AutoMapper;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.DTOs;
using Library.API.DTOs.Response;

using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

public class BooksController : ApiController
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public BooksController(IBookService bookService, IMapper mapper)
    {
        _bookService = bookService;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookDto>>), 200)]
    [ProducesResponseType(typeof(ApiProblemDetails), 400)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _bookService.ListAsync();
        if (!result.Success)
            return HandleErrorResponse(result);

        var booksDto = _mapper.Map<IEnumerable<BookDto>>(result.Model);
        return Success(booksDto);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), 200)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _bookService.FindByIdAsync(id);
        if (!result.Success)
            return HandleErrorResponse(result);

        var bookDto = _mapper.Map<BookDto>(result.Model);
        return Success(bookDto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), 201)]
    [ProducesResponseType(typeof(ApiProblemDetails), 400)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> CreateAsync([FromBody] SaveBookDto saveBookDto)
    {
        var book = _mapper.Map<Book>(saveBookDto);
        var result = await _bookService.AddAsync(book);
        if (!result.Success)
            return HandleErrorResponse(result);

        var bookDto = _mapper.Map<BookDto>(result.Model);
        return Created(nameof(GetByIdAsync), new { id = bookDto.Id }, bookDto);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookDto>), 200)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SaveBookDto saveBookDto)
    {
        var book = _mapper.Map<Book>(saveBookDto);
        var result = await _bookService.UpdateAsync(id, book);
        if (!result.Success)
            return HandleErrorResponse(result);

        var bookDto = _mapper.Map<BookDto>(result.Model);
        return Success(bookDto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _bookService.DeleteAsync(id);
        if (!result.Success)
            return HandleErrorResponse(result);

        return NoContent();
    }
}
