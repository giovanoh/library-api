using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Library.API.Infrastructure.Factories;

public static class InvalidModelStateResponseFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var modelStateEntry in context.ModelState)
        {
            var errorMessages = new List<string>();
            foreach (ModelError error in modelStateEntry.Value.Errors)
                errorMessages.Add(error.ErrorMessage);

            if (errorMessages.Count > 0)
            {
                errors.Add(modelStateEntry.Key, errorMessages.ToArray());
            }
        }

        var instance = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
        var problemDetails = ApiProblemDetailsFactory.CreateValidationProblem(errors, instance);

        return new BadRequestObjectResult(problemDetails);
    }
} 