using AutoMapper;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.DTOs;
using Library.API.DTOs.Response;

using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

/// <summary>
/// Controller responsible for managing author-related operations
/// </summary>
public class AuthorsController : ApiController
{
    private readonly IAuthorService _authorService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the AuthorsController
    /// </summary>
    /// <param name="authorService">Service for handling author operations</param>
    /// <param name="mapper">Object mapping service</param>
    public AuthorsController(IAuthorService authorService, IMapper mapper)
    {
        _authorService = authorService;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all registered authors
    /// </summary>
    /// <returns>List of authors</returns>
    /// <response code="200">Returns the list of authors</response>
    /// <response code="400">Validation Error</response>
    /// <response code="500">Internal Server Error</response>
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

    /// <summary>
    /// Retrieves a specific author by their unique identifier
    /// </summary>
    /// <param name="id">Unique identifier of the author</param>
    /// <returns>Author details</returns>
    /// <response code="200">Returns the author details</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error</response>
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

    /// <summary>
    /// Creates a new author
    /// </summary>
    /// <param name="saveAuthorDto">Data for creating the author</param>
    /// <returns>Created author</returns>
    /// <response code="201">Author created successfully</response>
    /// <response code="400">Validation Error</response>
    /// <response code="500">Internal Server Error</response>
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

    /// <summary>
    /// Updates an existing author's data
    /// </summary>
    /// <param name="id">Identifier of the author to be updated</param>
    /// <param name="saveAuthorDto">New author data</param>
    /// <returns>Updated author</returns>
    /// <response code="200">Author updated successfully</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error</response>
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

    /// <summary>
    /// Removes an author by their identifier
    /// </summary>
    /// <param name="id">Identifier of the author to be removed</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Author removed successfully</response>
    /// <response code="404">Resource Not Found</response>
    /// <response code="500">Internal Server Error</response>
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
