using Library.API.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    protected IActionResult HandleErrorResponse<T>(Response<T> response)
    {
        return response.Error switch
        {
            ErrorType.NotFound => NotFound(response.Message),
            ErrorType.ValidationError => BadRequest(response.Message),
            ErrorType.Conflict => Conflict(response.Message),
            ErrorType.DatabaseError => StatusCode(500, response.Message),
            _ => BadRequest(response.Message)
        };
    }
}