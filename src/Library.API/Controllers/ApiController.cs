using Microsoft.AspNetCore.Mvc;

using Library.API.Domain.Services.Communication;
using Library.API.DTOs.Response;
using Library.API.Infrastructure.Factories;

namespace Library.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult Success<T>(T data, int statusCode = 200, Dictionary<string, object>? meta = null)
    {
        var response = ApiResponse<T>.Ok(data, meta);
        return StatusCode(statusCode, response);
    }

    protected IActionResult Created<T>(string routeName, object routeValues, T data)
    {
        var response = ApiResponse<T>.Ok(data);
        return CreatedAtRoute(routeName, routeValues, response);
    }

    protected IActionResult Error(ApiProblemDetails problemDetails)
    {
        return StatusCode(problemDetails.Status, problemDetails);
    }

    protected IActionResult HandleErrorResponse<T>(Response<T> response)
    {
        var instance = $"{Request.Path}{Request.QueryString}";

        ApiProblemDetails problemDetails = response.Error switch
        {
            ErrorType.ValidationError => ApiProblemDetailsFactory.Create(
                StatusCodes.Status400BadRequest,
                "Validation Error",
                response.Message ?? "The request is invalid",
                instance),
            ErrorType.NotFound => ApiProblemDetailsFactory.CreateNotFound(
                response.Message ?? "Resource not found",
                instance),
            ErrorType.Conflict => ApiProblemDetailsFactory.Create(
                StatusCodes.Status409Conflict,
                "Conflict",
                response.Message ?? "A conflict occurred while processing the request",
                instance),
            ErrorType.DatabaseError => ApiProblemDetailsFactory.CreateInternalServerError(
                response.Message ?? "An error occurred while processing the database operation",
                instance),
            _ => ApiProblemDetailsFactory.Create(
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                response.Message ?? "An error occurred while processing the request",
                instance)
        };

        return Error(problemDetails);
    }
}