using System.Diagnostics;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.DTOs;
using Library.API.DTOs.Response;

namespace Library.API.Controllers;

/// <summary>
/// Controller responsible for managing book orders
/// </summary>
public class CheckoutController : ApiController
{
    private readonly IBookOrderService _bookOrderService;
    private readonly IMapper _mapper;
    private readonly ActivitySource _activitySource;

    public CheckoutController(IBookOrderService bookOrderService, IMapper mapper, ActivitySource activitySource)
    {
        _bookOrderService = bookOrderService;
        _mapper = mapper;
        _activitySource = activitySource;
    }

    /// <summary>
    /// Gets a book order by its ID
    /// </summary>
    /// <param name="id">The ID of the book order to get</param>
    /// <returns>The book order</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookOrderDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BookOrderDto>), 404)]
    [ProducesResponseType(typeof(ApiResponse<BookOrderDto>), 500)]
    public async Task<IActionResult> GetBookOrder(int id)
    {
        using var activity = _activitySource.StartActivity("Controller: CheckoutController.GetBookOrder");
        var result = await _bookOrderService.FindByIdAsync(id);

        if (!result.Success)
            return HandleErrorResponse(result);

        var bookOrderDto = _mapper.Map<BookOrderDto>(result.Model);
        return Success(bookOrderDto);
    }

    /// <summary>
    /// Creates a new book order
    /// </summary>
    /// <param name="bookOrder">The book order to be created</param>
    /// <returns>The created book order</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookOrderDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BookOrderDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<BookOrderDto>), 500)]
    public async Task<IActionResult> CreateBookOrder([FromBody] SaveBookOrderDto bookOrder)
    {
        using var activity = _activitySource.StartActivity("Controller: CheckoutController.CreateBookOrder");
        var order = _mapper.Map<BookOrder>(bookOrder);
        var result = await _bookOrderService.AddAsync(order);

        if (!result.Success)
            return HandleErrorResponse(result);

        var bookOrderDto = _mapper.Map<BookOrderDto>(result.Model);
        return Success(bookOrderDto);
    }
}
