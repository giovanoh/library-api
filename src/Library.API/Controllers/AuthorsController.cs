using AutoMapper;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.DTOs;
using Library.API.DTOs.Response;

using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

public class AuthorsController : ApiController
{
    private readonly IAuthorService _authorService;
    private readonly IMapper _mapper;

    public AuthorsController(IAuthorService authorService, IMapper mapper)
    {
        _authorService = authorService;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AuthorDto>>), 200)]
    [ProducesResponseType(typeof(ApiProblemDetails), 400)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _authorService.ListAsync();
        if (!result.Success)   
            return HandleErrorResponse(result);

        var authorsDto = _mapper.Map<IEnumerable<AuthorDto>>(result.Model);
        return Success(authorsDto);
    }

    [HttpGet("{id}", Name = "GetAuthorById")]
    [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 200)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _authorService.FindByIdAsync(id);
        if (!result.Success)
            return HandleErrorResponse(result);

        var authorDto = _mapper.Map<AuthorDto>(result.Model);
        return Success(authorDto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 201)]
    [ProducesResponseType(typeof(ApiProblemDetails), 400)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> CreateAsync([FromBody] SaveAuthorDto saveAuthorDto)
    {
        var author = _mapper.Map<Author>(saveAuthorDto);
        var result = await _authorService.AddAsync(author);
        
        if (!result.Success)
            return HandleErrorResponse(result);

        var authorDto = _mapper.Map<AuthorDto>(result.Model);
        return Created("GetAuthorById", new { id = authorDto.Id }, authorDto);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 200)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SaveAuthorDto saveAuthorDto)
    {
        var author = _mapper.Map<Author>(saveAuthorDto);
        var result = await _authorService.UpdateAsync(id, author);
        
        if (!result.Success)
            return HandleErrorResponse(result);

        var authorDto = _mapper.Map<AuthorDto>(result.Model);
        return Success(authorDto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiProblemDetails), 404)]
    [ProducesResponseType(typeof(ApiProblemDetails), 500)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _authorService.DeleteAsync(id);
        
        if (!result.Success)
            return HandleErrorResponse(result);

        return NoContent();
    }
}
