using Microsoft.AspNetCore.Mvc;
using Library.API.DTOs.Response;

namespace Library.API.Infrastructure.Factories;

public static class InvalidModelStateResponseFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var errorResponse = new ValidationErrorResponse();

        foreach (var modelStateEntry in context.ModelState)
        {
            foreach (var error in modelStateEntry.Value.Errors)
            {
                errorResponse.Errors.Add(new ValidationError
                {
                    Field = modelStateEntry.Key,
                    Message = error.ErrorMessage
                });
            }
        }

        return new BadRequestObjectResult(errorResponse);
    }
} 