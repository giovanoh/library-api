using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Library.API.DTOs.Response;
using Library.API.Infrastructure.Factories;

namespace Library.API.Tests.Factories;

public class InvalidModelStateResponseFactoryTests
{
    [Fact]
    public void Create_WithSingleModelError_ShouldReturnBadRequestWithCorrectProblemDetails()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Title", "Title is required");

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/books";
        httpContext.Request.QueryString = QueryString.Create("test", "value");

        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ActionDescriptor()
        );
        actionContext.ModelState.Merge(modelState);

        // Act
        var result = InvalidModelStateResponseFactory.Create(actionContext) as BadRequestObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);

        var problemDetails = result.Value as ApiProblemDetails;
        problemDetails.Should().NotBeNull();

        problemDetails.Errors.Should().ContainKey("Title");
        problemDetails.Errors["Title"].Should().Contain("Title is required");
        problemDetails.Instance.Should().Be("/api/books?test=value");
    }

    [Fact]
    public void Create_WithMultipleModelErrors_ShouldReturnBadRequestWithAllErrors()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Title", "Title is required");
        modelState.AddModelError("Author", "Author cannot be empty");

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/books";

        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ActionDescriptor()
        );
        actionContext.ModelState.Merge(modelState);

        // Act
        var result = InvalidModelStateResponseFactory.Create(actionContext) as BadRequestObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);

        var problemDetails = result.Value as ApiProblemDetails;
        problemDetails.Should().NotBeNull();

        problemDetails.Errors.Should().ContainKeys("Title", "Author");
        problemDetails.Errors["Title"].Should().Contain("Title is required");
        problemDetails.Errors["Author"].Should().Contain("Author cannot be empty");
    }

    [Fact]
    public void Create_WithNoModelErrors_ShouldReturnEmptyErrorDictionary()
    {
        // Arrange
        var modelState = new ModelStateDictionary();

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/books";

        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ActionDescriptor()
        );
        actionContext.ModelState.Merge(modelState);

        // Act
        var result = InvalidModelStateResponseFactory.Create(actionContext) as BadRequestObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);

        var problemDetails = result.Value as ApiProblemDetails;
        problemDetails.Should().NotBeNull();

        problemDetails.Errors.Should().BeEmpty();
    }
}