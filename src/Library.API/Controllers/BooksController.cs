using AutoMapper;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.DTOs;

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
    [ProducesResponseType(typeof(IEnumerable<BookDto>), 200)]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _bookService.ListAsync();
        if (!result.Success)
            return HandleErrorResponse(result);

        var booksDto = _mapper.Map<IEnumerable<BookDto>>(result.Model);
        return Ok(booksDto);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDto), 200)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _bookService.FindByIdAsync(id);
        if (!result.Success)
            return HandleErrorResponse(result);

        var bookDto = _mapper.Map<BookDto>(result.Model);
        return Ok(bookDto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookDto), 201)]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> CreateAsync([FromBody] SaveBookDto saveBookDto)
    {
        var book = _mapper.Map<Book>(saveBookDto);
        var result = await _bookService.AddAsync(book);
        if (!result.Success)
            return HandleErrorResponse(result);

        var bookDto = _mapper.Map<BookDto>(result.Model);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = bookDto.Id }, bookDto);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BookDto), 200)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SaveBookDto saveBookDto)
    {
        var book = _mapper.Map<Book>(saveBookDto);
        var result = await _bookService.UpdateAsync(id, book);
        if (!result.Success)
            return HandleErrorResponse(result);

        var bookDto = _mapper.Map<BookDto>(result.Model);
        return Ok(bookDto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(NoContentResult), 204)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _bookService.DeleteAsync(id);
        if (!result.Success)
            return HandleErrorResponse(result);

        return NoContent();
    }
}
