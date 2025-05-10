using Library.API.DTOs.Response;

namespace Library.API.Infrastructure.Factories;

public static class ApiProblemDetailsFactory
{
    public static ApiProblemDetails Create(int status, string title, string detail, string? instance = null)
    {
        return new ApiProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = instance
        };
    }

    public static ApiProblemDetails CreateValidationProblem(IDictionary<string, string[]> errors, string? instance = null)
    {
        return new ApiProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Validation Error",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = instance,
            Errors = new Dictionary<string, string[]>(errors)
        };
    }

    public static ApiProblemDetails CreateNotFound(string detail, string? instance = null)
    {
        return new ApiProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Resource Not Found",
            Status = StatusCodes.Status404NotFound,
            Detail = detail,
            Instance = instance
        };
    }

    public static ApiProblemDetails CreateInternalServerError(string detail, string? instance = null)
    {
        return new ApiProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = detail,
            Instance = instance
        };
    }
} 