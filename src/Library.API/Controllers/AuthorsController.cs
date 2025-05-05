using AutoMapper;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.DTOs;

using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

public class AuthorsController : ApiController
{
    private readonly IAuthorService _authorService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthorsController> _logger;
    public AuthorsController(IAuthorService authorService, IMapper mapper, ILogger<AuthorsController> logger)
    {
        _authorService = authorService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuthorDto>), 200)]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _authorService.ListAsync();
        if (!result.Success)   
            return HandleErrorResponse(result);

        var authorsDto = _mapper.Map<IEnumerable<AuthorDto>>(result.Model);
        return Ok(authorsDto);
    }

    [HttpGet("{id}", Name = "GetAuthorById")]
    [ProducesResponseType(typeof(AuthorDto), 200)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _authorService.FindByIdAsync(id);
        if (!result.Success)
            return HandleErrorResponse(result);

        var authorDto = _mapper.Map<AuthorDto>(result.Model);
        return Ok(authorDto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AuthorDto), 201)]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> CreateAsync([FromBody] SaveAuthorDto saveAuthorDto)
    {
        var author = _mapper.Map<Author>(saveAuthorDto);
        var result = await _authorService.AddAsync(author);
        
        if (!result.Success)
            return HandleErrorResponse(result);

        var authorDto = _mapper.Map<AuthorDto>(result.Model);
        return CreatedAtRoute("GetAuthorById", new { id = authorDto.Id }, authorDto);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AuthorDto), 200)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SaveAuthorDto saveAuthorDto)
    {
        var author = _mapper.Map<Author>(saveAuthorDto);
        var result = await _authorService.UpdateAsync(id, author);
        
        if (!result.Success)
            return HandleErrorResponse(result);

        var authorDto = _mapper.Map<AuthorDto>(result.Model);
        return Ok(authorDto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(AuthorDto), 200)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
    [ProducesResponseType(typeof(ObjectResult), 500)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _authorService.DeleteAsync(id);
        
        if (!result.Success)
            return HandleErrorResponse(result);

        var authorDto = _mapper.Map<AuthorDto>(result.Model);
        return Ok(authorDto);
    }
}
