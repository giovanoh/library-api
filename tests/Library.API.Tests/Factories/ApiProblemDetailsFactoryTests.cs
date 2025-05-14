using System.ComponentModel.DataAnnotations;

using FluentAssertions;
using Microsoft.AspNetCore.Http;

using Library.API.DTOs;
using Library.API.DTOs.Response;
using Library.API.Infrastructure.Factories;

namespace Library.API.Tests.Factories;

public class ApiProblemDetailsFactoryTests
{
    [Fact]
    public void Create_ShouldReturnApiProblemDetails_WhenBadRequest()
    {
        // Arrange
        var statusCode = StatusCodes.Status400BadRequest;
        var type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        var detail = "One or more validation errors occurred.";
        var instance = "/api/books";

        // Act
        var result = ApiProblemDetailsFactory.Create(statusCode, type, detail, instance);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiProblemDetails>();
        result.Type.Should().Be(type);
        result.Title.Should().Be(type);
        result.Status.Should().Be(statusCode);
        result.Detail.Should().Be(detail);
        result.Instance.Should().Be(instance);
        result.Errors.Should().BeNull();
        result.CorrelationId.Should().NotBeNull();
    }

    [Fact]
    public void CreateValidationProblem_ShouldReturnApiProblemDetails_WhenBookValidationErrors()
    {
        // Arrange
        var bookDto = new SaveBookDto
        {
            Title = new string('A', 101),
            Description = new string('B', 1001),
        };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(bookDto);
        var isValid = Validator.TryValidateObject(bookDto, validationContext, validationResults, true);

        var errors = validationResults
            .GroupBy(vr => vr.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                g => g.Key,
                g => g.Select(vr => vr.ErrorMessage ?? "Validation error").ToArray()
            );

        // Act
        var result = ApiProblemDetailsFactory.CreateValidationProblem(errors);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiProblemDetails>();
        result.Status.Should().Be(400);
        result.Title.Should().Be("Validation Error");
        result.Detail.Should().Be("One or more validation errors occurred.");
        result.CorrelationId.Should().NotBeNull();
        result.CorrelationId.Should().NotBeEmpty();
        result.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");
        result.Instance.Should().BeNull();

        result.Errors.Should().NotBeNull();
        result.Errors.Should().ContainKeys("Title", "Description", "AuthorId");
        result.Errors["Title"].Should().Contain("The field Title must be a string or array type with a maximum length of '100'.");
        result.Errors["Description"].Should().Contain("The field Description must be a string or array type with a maximum length of '1000'.");
        result.Errors["AuthorId"].Should().Contain("The AuthorId field is required.");
    }

    [Fact]
    public void CreateValidationProblem_ShouldReturnApiProblemDetails_WhenAuthorValidationErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Custom name error" } },
            { "BirthDate", new[] { "Custom birthdate error" } },
            { "Biography", new[] { "Custom biography error" } }
        };

        // Act
        var result = ApiProblemDetailsFactory.CreateValidationProblem(errors);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiProblemDetails>();
        result.Status.Should().Be(400);
        result.Title.Should().Be("Validation Error");
        result.Detail.Should().Be("One or more validation errors occurred.");
        result.CorrelationId.Should().NotBeNull();
        result.CorrelationId.Should().NotBeEmpty();
        result.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");
        result.Instance.Should().BeNull();

        result.Errors.Should().NotBeNull();
        result.Errors.Should().ContainKeys("Name", "BirthDate", "Biography");
        result.Errors["Name"].Should().Contain("Custom name error");
        result.Errors["BirthDate"].Should().Contain("Custom birthdate error");
        result.Errors["Biography"].Should().Contain("Custom biography error");
    }

    [Fact]
    public void CreateNotFound_ShouldReturnApiProblemDetails_WhenResourceMissing()
    {
        // Arrange
        string detail = "Book not found in the library catalog";
        string? instance = "/api/books/123";

        // Act
        var result = ApiProblemDetailsFactory.CreateNotFound(detail, instance);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiProblemDetails>();
        result.Status.Should().Be(404);
        result.Title.Should().Be("Resource Not Found");
        result.Detail.Should().Be(detail);
        result.CorrelationId.Should().NotBeNull();
        result.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.4");
        result.Instance.Should().Be(instance);
        result.Errors.Should().BeNull();
    }

    [Fact]
    public void CreateInternalServerError_ShouldReturnApiProblemDetails_WhenServerFailure()
    {
        // Arrange
        string detail = "Unexpected error occurred while processing the request";
        string? instance = "/api/books";

        // Act
        var result = ApiProblemDetailsFactory.CreateInternalServerError(detail, instance);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiProblemDetails>();
        result.Status.Should().Be(500);
        result.Title.Should().Be("Internal Server Error");
        result.Detail.Should().Be(detail);
        result.CorrelationId.Should().NotBeNull();
        result.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.6.1");
        result.Instance.Should().Be(instance);
        result.Errors.Should().BeNull();
    }

    [Fact]
    public void CreateValidationProblem_ShouldReturnApiProblemDetails_WhenErrorsProvided()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Title", new[] { "Book title is required" } },
            { "AuthorId", new[] { "Author must be specified" } }
        };

        // Act
        var result = ApiProblemDetailsFactory.CreateValidationProblem(errors);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiProblemDetails>();
        result.Status.Should().Be(400);
        result.Title.Should().Be("Validation Error");
        result.Detail.Should().Be("One or more validation errors occurred.");
        result.CorrelationId.Should().NotBeNull();
        result.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");
        result.Instance.Should().BeNull();

        result.Errors.Should().NotBeNull();
        result.Errors.Should().ContainKeys("Title", "AuthorId");
        result.Errors["Title"].Should().Contain("Book title is required");
        result.Errors["AuthorId"].Should().Contain("Author must be specified");
    }
}