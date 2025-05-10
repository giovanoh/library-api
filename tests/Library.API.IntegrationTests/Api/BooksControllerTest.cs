using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Library.API.DTOs;
using Library.API.DTOs.Response;
using Library.API.IntegrationTests.Fixtures;

namespace Library.API.IntegrationTests.Api;

public class BooksControllerTest(LibraryApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnSuccess()
    {
        // Arrange
        var endpoint = "/api/books";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnBooks()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        TestDataHelper.SeedBooks(Factory);
        var endpoint = "/api/books";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<BookDto>>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBook()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        TestDataHelper.SeedBooks(Factory);
        const int bookId = 1;
        var endpoint = $"/api/books/{bookId}";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Id.Should().Be(bookId);
        apiResponse.Data.Title.Should().Be("Book 1");
        apiResponse.Data.Description.Should().Be("Book 1 is a book about software engineering.");
        apiResponse.Data.ReleaseDate.Should().Be(new DateTime(2000, 1, 1));
        apiResponse.Data.AuthorId.Should().Be(1);
        apiResponse.Data.AuthorName.Should().Be("Author 1");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var endpoint = $"/api/books/1001";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        var endpoint = "/api/books";
        var newBook = new SaveBookDto
        {
            Title = "New Book",
            Description = "New Book Description",
            ReleaseDate = new DateTime(2020, 1, 1),
            AuthorId = 1
        };

        // Act
        var response = await Client.PostAsJsonAsync(endpoint, newBook, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>(CancellationToken);
        var locationHeader = response.Headers.Location;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Id.Should().BeGreaterThan(0);
        apiResponse.Data.Title.Should().Be(newBook.Title);
        apiResponse.Data.Description.Should().Be(newBook.Description);
        apiResponse.Data.ReleaseDate.Should().Be(newBook.ReleaseDate);
        apiResponse.Data.AuthorId.Should().Be(newBook.AuthorId);
        apiResponse.Data.AuthorName.Should().Be("Author 1");

        locationHeader.Should().NotBeNull();
        locationHeader.ToString().Should().EndWith($"{endpoint}/{apiResponse.Data.Id}");
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var endpoint = "/api/books";
        var newBook = new SaveBookDto
        {
            Title = "",
            Description = "New Book Description",
            ReleaseDate = new DateTime(2020, 1, 1)
        };

        // Act
        var response = await Client.PostAsJsonAsync(endpoint, newBook, CancellationToken);
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
        apiResponse.Errors.Should().ContainKey("Title");
        apiResponse.Errors["Title"].Should().Contain("The Title field is required.");
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldReturnUpdatedBook()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        TestDataHelper.SeedBooks(Factory);
        const int bookId = 1;
        var endpoint = $"/api/books/{bookId}";
        var updatedBook = new SaveBookDto
        {
            Title = "Updated Book",
            Description = "Updated Book Description",
            ReleaseDate = new DateTime(2021, 1, 1),
            AuthorId = 1
        };

        // Act
        var response = await Client.PutAsJsonAsync(endpoint, updatedBook, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Id.Should().Be(bookId);
        apiResponse.Data.Title.Should().Be(updatedBook.Title);
        apiResponse.Data.Description.Should().Be(updatedBook.Description);
        apiResponse.Data.ReleaseDate.Should().Be(updatedBook.ReleaseDate);
        apiResponse.Data.AuthorId.Should().Be(updatedBook.AuthorId);
        apiResponse.Data.AuthorName.Should().Be("Author 1");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        var endpoint = $"/api/books/1001";
        var updatedBook = new SaveBookDto
        {
            Title = "Updated Book",
            Description = "Updated Book Description",
            ReleaseDate = new DateTime(2021, 1, 1),
            AuthorId = 1
        };

        // Act
        var response = await Client.PutAsJsonAsync(endpoint, updatedBook, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Resource Not Found");
        apiResponse.Status.Should().Be((int)HttpStatusCode.NotFound);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.4");
        apiResponse.Detail.Should().Be("Book with id 1001 was not found");
        apiResponse.Instance.Should().Be(endpoint);
        apiResponse.Errors.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        TestDataHelper.SeedBooks(Factory);
        const int bookId = 1;
        var endpoint = $"/api/books/{bookId}";
        var updatedBook = new SaveBookDto
        {
            Title = "",
            Description = "Updated Book Description",
            ReleaseDate = new DateTime(2021, 1, 1),
            AuthorId = 0
        };

        // Act
        var response = await Client.PutAsJsonAsync(endpoint, updatedBook, CancellationToken);
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
        apiResponse.Errors.Should().ContainKey("Title");
        apiResponse.Errors["Title"].Should().Contain("The Title field is required.");
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        TestDataHelper.SeedBooks(Factory);
        const int bookId = 1;
        var endpoint = $"/api/books/{bookId}";

        // Act
        var response = await Client.DeleteAsync(endpoint, CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var endpoint = $"/api/books/1001";

        // Act
        var response = await Client.DeleteAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Resource Not Found");
        apiResponse.Status.Should().Be((int)HttpStatusCode.NotFound);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.4");
        apiResponse.Detail.Should().Be("Book with id 1001 was not found");
        apiResponse.Instance.Should().Be(endpoint);
        apiResponse.Errors.Should().BeNull();
    }
}