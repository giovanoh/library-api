using AutoMapper;

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
    public async Task<IActionResult> GetAllAsync()
    {
        var authors = await _authorService.ListAsync();
        var authorsDto = _mapper.Map<IEnumerable<AuthorDto>>(authors);
        return Ok(authorsDto);
    }
}
