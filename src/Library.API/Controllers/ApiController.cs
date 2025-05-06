using Library.API.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult HandleErrorResponse<T>(Response<T> response)
    {
        var errorObject = new { message = response.Message };
        return response.Error switch
        {
            ErrorType.NotFound => NotFound(errorObject),
            ErrorType.ValidationError => BadRequest(errorObject),
            ErrorType.Conflict => Conflict(errorObject),
            ErrorType.DatabaseError => StatusCode(500, errorObject),
            _ => BadRequest(errorObject)
        };
    }
}