using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Library.API.DTOs;
using Library.API.DTOs.Response;
using Library.API.IntegrationTests.Fixtures;
using Library.Events.Messages;

namespace Library.API.IntegrationTests.Api;

public class CheckoutControllerTest(LibraryApiFactory factory) : IntegrationTestBase(factory)
{

    [Fact]
    public async Task CreateBookOrder_ShouldReturnSuccess_AndPublishOrderPlacedEvent()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        TestDataHelper.SeedBooks(Factory);
        var endpoint = "/api/checkout";
        var newBookOrder = new SaveBookOrderDto
        {
            Items = [
                new SaveBookOrderItemDto { BookId = 1, Quantity = 1 }
            ]
        };

        // Act
        var response = await Client.PostAsJsonAsync(endpoint, newBookOrder, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookOrderDto>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Id.Should().Be(1);
        apiResponse.Data.Items.Should().HaveCount(1);
        apiResponse.Data.Items[0].BookId.Should().Be(1);
        apiResponse.Data.Items[0].Quantity.Should().Be(1);

        VerifyEventPublished<OrderPlacedEvent>();
    }

    [Fact]
    public async Task CreateBookOrder_WithInvalidData_ShouldReturnBadRequest_AndNotPublishEvent()
    {
        // Arrange
        var endpoint = "/api/checkout";
        var newBookOrder = new SaveBookOrderDto { Items = [] };

        // Act
        var response = await Client.PostAsJsonAsync(endpoint, newBookOrder, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Validation Error");
        apiResponse.Status.Should().Be((int)HttpStatusCode.BadRequest);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");
        apiResponse.Detail.Should().Be("Book order must have at least one item");
        apiResponse.Instance.Should().Be(endpoint);

        VerifyEventNotPublished<OrderPlacedEvent>();
    }

    [Fact]
    public async Task CreateBookOrder_WithInvalidBookId_ShouldReturnBadRequest()
    {
        // Arrange
        var endpoint = "/api/checkout";
        var newBookOrder = new SaveBookOrderDto
        {
            Items = [
                new SaveBookOrderItemDto { BookId = 999, Quantity = 1 }
            ]
        };

        // Act
        var response = await Client.PostAsJsonAsync(endpoint, newBookOrder, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Validation Error");
        apiResponse.Status.Should().Be((int)HttpStatusCode.BadRequest);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");
        apiResponse.Detail.Should().Be("Book with id 999 was not found");
        apiResponse.Instance.Should().Be(endpoint);

        VerifyEventNotPublished<OrderPlacedEvent>();
    }

    [Fact]
    public async Task CreateBookOrder_WithInvalidQuantity_ShouldReturnBadRequest()
    {
        // Arrange
        var endpoint = "/api/checkout";
        var newBookOrder = new SaveBookOrderDto
        {
            Items = [
                new SaveBookOrderItemDto { BookId = 1 }
            ]
        };

        // Act
        var response = await Client.PostAsJsonAsync(endpoint, newBookOrder, CancellationToken);
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
        apiResponse.Errors.Should().ContainKey("Items[0].Quantity");
        apiResponse.Errors["Items[0].Quantity"].Should().Contain("The field Quantity must be between 1 and 50.");

        VerifyEventNotPublished<OrderPlacedEvent>();
    }

    [Fact]
    public async Task GetBookOrder_WithValidId_ShouldReturnBookOrder()
    {
        // Arrange
        TestDataHelper.SeedAuthors(Factory, true);
        TestDataHelper.SeedBooks(Factory);
        TestDataHelper.SeedBookOrders(Factory);
        var orderId = 1;
        var endpoint = $"/api/checkout/{orderId}";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookOrderDto>>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Id.Should().Be(orderId);
        apiResponse.Data.CheckoutDate.Should().Be(new DateTime(2020, 1, 1));
        apiResponse.Data.Status.Should().Be("Order Placed");
        apiResponse.Data.Items.Should().HaveCount(1);
        apiResponse.Data.Items[0].BookId.Should().Be(1);
        apiResponse.Data.Items[0].Title.Should().Be("Book 1");
        apiResponse.Data.Items[0].Quantity.Should().Be(1);

        VerifyEventNotPublished<OrderPlacedEvent>();
    }

    [Fact]
    public async Task GetBookOrder_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var endpoint = "/api/checkout/999";

        // Act
        var response = await Client.GetAsync(endpoint, CancellationToken);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        apiResponse.Should().NotBeNull();
        apiResponse.Title.Should().Be("Resource Not Found");
        apiResponse.Status.Should().Be((int)HttpStatusCode.NotFound);
        apiResponse.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.4");
        apiResponse.Detail.Should().Be("Book order with id 999 was not found");
        apiResponse.Instance.Should().Be(endpoint);

        VerifyEventNotPublished<OrderPlacedEvent>();
    }

}