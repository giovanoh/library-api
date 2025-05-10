using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Library.API.DTOs;
using Library.API.DTOs.Response;
using Library.API.IntegrationTests.Fixtures;

namespace Library.API.IntegrationTests.Api;

public class AuthorsControllerTest(LibraryApiFactory factory) : IntegrationTestBase(factory)
{

    [Fact]
    public async Task GetAllAsync_ShouldReturnSuccess()
    {
        // Arrange
        var endpoint = "/api/authors";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAuthors()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        var endpoint = "/api/authors";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<AuthorDto>>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnAuthor()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        const int authorId = 1;
        var endpoint = $"/api/authors/{authorId}";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthorDto>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Id.Should().Be(authorId);
        apiResponse.Data.Name.Should().Be("Author 1");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var endpoint = $"/api/authors/1001";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthorDto>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var endpoint = "/api/authors";
        var newAuthor = new SaveAuthorDto
        {
            Name = "New Author",
            Biography = "New Author Biography",
            BirthDate = new DateTime(1995, 1, 1),
        };

        // Act
        var response = await Client.PostAsJsonAsync(endpoint, newAuthor, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthorDto>>(CancellationToken);
        var locationHeader = response.Headers.Location;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Name.Should().Be(newAuthor.Name);
        apiResponse.Data.Biography.Should().Be(newAuthor.Biography);
        apiResponse.Data.BirthDate.Should().Be(newAuthor.BirthDate);

        locationHeader.Should().NotBeNull();
        locationHeader.ToString().Should().EndWith($"{endpoint}/{apiResponse.Data.Id}");
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var endpoint = "/api/authors";
        var newAuthor = new SaveAuthorDto
        {
            Name = "",
            Biography = "New Author Biography",
            BirthDate = new DateTime(1995, 1, 1),
        };

        // Act
        var response = await Client.PostAsJsonAsync(endpoint, newAuthor, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Validation Error");
        apiResponse.Status.Should().Be((int)HttpStatusCode.BadRequest);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");
        apiResponse.Detail.Should().Be("One or more validation errors occurred.");
        apiResponse.Instance.Should().Be(endpoint);
        apiResponse.Errors.Should().NotBeNull();
        apiResponse.Errors.Should().HaveCount(1);
        apiResponse.Errors.Should().ContainKey("Name");
        apiResponse.Errors["Name"].Should().Contain("The Name field is required.");
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDate_ShouldReturnBadRequest()
    {
        // Arrange
        var endpoint = "/api/authors";
        var newAuthor = new SaveAuthorDto
        {
            Name = "New Author",
            Biography = "New Author Biography",
            BirthDate = DateTime.Today.AddDays(1),
        };

        // Act
        var response = await Client.PostAsJsonAsync(endpoint, newAuthor, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Validation Error");
        apiResponse.Status.Should().Be((int)HttpStatusCode.BadRequest);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");
        apiResponse.Detail.Should().Be("One or more validation errors occurred.");
        apiResponse.Instance.Should().Be(endpoint);
        apiResponse.Errors.Should().NotBeNull();
        apiResponse.Errors.Should().HaveCount(1);
        apiResponse.Errors.Should().ContainKey("BirthDate");
        apiResponse.Errors["BirthDate"].Should().Contain("The BirthDate cannot be in the future.");
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldReturnUpdatedAuthor()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        const int authorId = 1;
        var endpoint = $"/api/authors/{authorId}";
        var updatedAuthor = new SaveAuthorDto
        {
            Name = "Updated Author",
            Biography = "Updated Author Biography",
            BirthDate = new DateTime(1995, 1, 1),
        };

        // Act
        var response = await Client.PutAsJsonAsync(endpoint, updatedAuthor, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthorDto>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Name.Should().Be(updatedAuthor.Name);
        apiResponse.Data.Biography.Should().Be(updatedAuthor.Biography);
        apiResponse.Data.BirthDate.Should().Be(updatedAuthor.BirthDate);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        const int authorId = 1001;
        var endpoint = $"/api/authors/{authorId}";
        var updatedAuthor = new SaveAuthorDto
        {
            Name = "Updated Author",
            Biography = "Updated Author Biography",
            BirthDate = new DateTime(1995, 1, 1),
        };

        // Act
        var response = await Client.PutAsJsonAsync(endpoint, updatedAuthor, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Resource Not Found");
        apiResponse.Status.Should().Be((int)HttpStatusCode.NotFound);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.4");
        apiResponse.Detail.Should().Be($"Author with id {authorId} was not found");
        apiResponse.Instance.Should().Be(endpoint);
        apiResponse.Errors.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        const int authorId = 1;
        var endpoint = $"/api/authors/{authorId}";
        var updatedAuthor = new SaveAuthorDto
        {
            Name = "",
            Biography = "Updated Author Biography",
            BirthDate = new DateTime(1995, 1, 1),
        };

        // Act
        var response = await Client.PutAsJsonAsync(endpoint, updatedAuthor, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Validation Error");
        apiResponse.Status.Should().Be((int)HttpStatusCode.BadRequest);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");
        apiResponse.Detail.Should().Be("One or more validation errors occurred.");
        apiResponse.Instance.Should().Be(endpoint);
        apiResponse.Errors.Should().NotBeNull();
        apiResponse.Errors.Should().HaveCount(1);
        apiResponse.Errors.Should().ContainKey("Name");
        apiResponse.Errors["Name"].Should().Contain("The Name field is required.");
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        const int authorId = 1;
        var endpoint = $"/api/authors/{authorId}";

        // Act
        var response = await Client.DeleteAsync(endpoint, CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        const int authorId = 1001;
        var endpoint = $"/api/authors/{authorId}";

        // Act
        var response = await Client.DeleteAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Resource Not Found");
        apiResponse.Status.Should().Be((int)HttpStatusCode.NotFound);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.4");
        apiResponse.Detail.Should().Be($"Author with id {authorId} was not found");
        apiResponse.Instance.Should().Be(endpoint);
        apiResponse.Errors.Should().BeNull();
    }
}