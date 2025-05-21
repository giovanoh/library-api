using System.Diagnostics;

using AutoMapper;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;

using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Domain.Services.Communication;
using Library.API.Infrastructure.Services;
using Library.API.Tests.Helpers;
using Library.Events.Messages;

namespace Library.API.Tests.Services;

public class BookOrderServiceTests : ServiceTestBase<BookOrderService>
{
    private readonly Mock<IBookOrderRepository> _bookOrderRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ActivitySource _activitySource;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    public BookOrderServiceTests()
    {
        _bookOrderRepositoryMock = new Mock<IBookOrderRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _activitySource = new ActivitySource("Library.API.Tests");
        _mapperMock = new Mock<IMapper>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
    }

    private BookOrderService CreateService() =>
        new BookOrderService(
            _bookOrderRepositoryMock.Object,
            _bookRepositoryMock.Object,
            _unitOfWorkMock.Object,
            LoggerMock.Object,
            _activitySource,
            _mapperMock.Object,
            _publishEndpointMock.Object);

    [Fact]
    public async Task FindByIdAsync_ShouldReturnBookOrder_WhenSuccessful()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrderId = 1;
        var checkoutDate = new DateTime(2025, 5, 1);
        var bookOrder = new BookOrder
        {
            Id = bookOrderId,
            CheckoutDate = checkoutDate,
            Status = BookOrderStatus.Placed,
            Items =
            [
                new BookOrderItem
                {
                    Id = 1,
                    Book = TestDataHelper.Books[0],
                    Quantity = 1
                }
            ]
        };

        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ReturnsAsync(bookOrder);

        // Act
        var result = await bookOrderService.FindByIdAsync(bookOrderId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(bookOrder);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _bookOrderRepositoryMock.Verify(repo => repo.FindByIdAsync(bookOrderId), Times.Once);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnError_WhenBookOrderNotFound()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrderId = 1;
        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ReturnsAsync((BookOrder?)null);

        // Act
        var result = await bookOrderService.FindByIdAsync(bookOrderId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Book order with id {bookOrderId} was not found");

        _bookOrderRepositoryMock.Verify(repo => repo.FindByIdAsync(bookOrderId), Times.Once);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrderId = 1;
        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ThrowsAsync(new DbUpdateException("Repository error"));

        // Act
        var result = await bookOrderService.FindByIdAsync(bookOrderId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while retrieving the book order");

        _bookOrderRepositoryMock.Verify(repo => repo.FindByIdAsync(bookOrderId), Times.Once);
        VerifyErrorLog("Error occurred while finding book order");
    }

    [Fact]
    public async Task AddAsync_ShouldReturnBookOrder_WhenSuccessful()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrder = TestDataHelper.BookOrders[0];
        var bookOrderId = bookOrder.Id;
        var book = bookOrder.Items[0].Book;
        var bookId = book.Id;

        _bookOrderRepositoryMock.Setup(repo => repo.AddAsync(bookOrder)).Returns(Task.CompletedTask);
        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ReturnsAsync(bookOrder);
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(bookId)).ReturnsAsync(book);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);
        _publishEndpointMock.Setup(publishEndpoint => publishEndpoint.Publish(It.IsAny<OrderPlacedEvent>(), CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await bookOrderService.AddAsync(bookOrder);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(bookOrder);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _bookOrderRepositoryMock.Verify(repo => repo.AddAsync(bookOrder), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        _publishEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(It.IsAny<OrderPlacedEvent>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrder = TestDataHelper.BookOrders[0];
        var bookOrderId = bookOrder.Id;
        var book = bookOrder.Items[0].Book;
        var bookId = book.Id;

        _bookOrderRepositoryMock.Setup(repo => repo.AddAsync(bookOrder)).ThrowsAsync(new DbUpdateException("Repository error"));
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(bookId)).ReturnsAsync(book);

        // Act
        var result = await bookOrderService.AddAsync(bookOrder);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while saving the book order");

        _bookOrderRepositoryMock.Verify(repo => repo.AddAsync(bookOrder), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        _publishEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(It.IsAny<OrderPlacedEvent>(), CancellationToken.None), Times.Never);
        VerifyErrorLog("Error occurred while saving book order");
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenUnitOfWorkFails()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrder = TestDataHelper.BookOrders[0];
        var bookOrderId = bookOrder.Id;
        var book = bookOrder.Items[0].Book;
        var bookId = book.Id;

        _bookOrderRepositoryMock.Setup(repo => repo.AddAsync(bookOrder)).Returns(Task.CompletedTask);
        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ReturnsAsync(bookOrder);
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(bookId)).ReturnsAsync(book);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ThrowsAsync(new DbUpdateException("Unit of work error"));

        // Act
        var result = await bookOrderService.AddAsync(bookOrder);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while saving the book order");

        _bookOrderRepositoryMock.Verify(repo => repo.AddAsync(bookOrder), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        _publishEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(It.IsAny<OrderPlacedEvent>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrder = TestDataHelper.BookOrders[0];
        var bookOrderId = bookOrder.Id;
        var book = bookOrder.Items[0].Book;
        var bookId = book.Id;

        _bookOrderRepositoryMock.Setup(repo => repo.AddAsync(bookOrder)).Returns(Task.CompletedTask);
        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ReturnsAsync(bookOrder);
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(bookId)).ReturnsAsync(book);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await bookOrderService.AddAsync(bookOrder);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Should().Be(ErrorType.Unknown);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An unexpected error occurred while processing your request");

        _bookOrderRepositoryMock.Verify(repo => repo.AddAsync(bookOrder), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        _publishEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(It.IsAny<OrderPlacedEvent>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenBookOrderItemsAreEmpty()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrder = TestDataHelper.BookOrders[0];
        bookOrder.Items = [];

        // Act
        var result = await bookOrderService.AddAsync(bookOrder);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Should().Be(ErrorType.ValidationError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("Book order must have at least one item");

        _bookOrderRepositoryMock.Verify(repo => repo.AddAsync(bookOrder), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        _publishEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(It.IsAny<OrderPlacedEvent>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenPublishFails()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrder = TestDataHelper.BookOrders[0];
        var bookOrderId = bookOrder.Id;
        var book = bookOrder.Items[0].Book;
        var bookId = book.Id;

        _bookOrderRepositoryMock.Setup(repo => repo.AddAsync(bookOrder)).Returns(Task.CompletedTask);
        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ReturnsAsync(bookOrder);
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(bookId)).ReturnsAsync(book);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);
        _publishEndpointMock
            .Setup(publishEndpoint => publishEndpoint.Publish(It.IsAny<OrderPlacedEvent>(), CancellationToken.None))
            .ThrowsAsync(new MessageException());

        // Act
        var result = await bookOrderService.AddAsync(bookOrder);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Should().Be(ErrorType.MessageBrokerError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while publishing the order event");

        _bookOrderRepositoryMock.Verify(repo => repo.AddAsync(bookOrder), Times.Once);
        _bookOrderRepositoryMock.Verify(repo => repo.FindByIdAsync(bookOrderId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        _publishEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(It.IsAny<OrderPlacedEvent>(), CancellationToken.None), Times.Once);
        VerifyErrorLog("Error occurred while publishing order placed event");
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnBookOrder_WhenSuccessful()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrder = TestDataHelper.BookOrders[0];
        var bookOrderId = bookOrder.Id;
        var newStatus = BookOrderStatus.Processing;

        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ReturnsAsync(bookOrder);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await bookOrderService.UpdateStatusAsync(bookOrderId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Status.Should().Be(newStatus);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _bookOrderRepositoryMock.Verify(repo => repo.FindByIdAsync(bookOrderId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnError_WhenBookOrderNotFound()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrderId = 999;
        var newStatus = BookOrderStatus.Processing;

        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ReturnsAsync((BookOrder?)null);

        // Act
        var result = await bookOrderService.UpdateStatusAsync(bookOrderId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Book order with id {bookOrderId} was not found");

        _bookOrderRepositoryMock.Verify(repo => repo.FindByIdAsync(bookOrderId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        var bookOrderService = CreateService();
        var bookOrder = TestDataHelper.BookOrders[0];
        var bookOrderId = bookOrder.Id;
        var newStatus = BookOrderStatus.Processing;

        _bookOrderRepositoryMock.Setup(repo => repo.FindByIdAsync(bookOrderId)).ReturnsAsync(bookOrder);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ThrowsAsync(new Exception("Repository error"));

        // Act
        var result = await bookOrderService.UpdateStatusAsync(bookOrderId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while updating the book order status");

        _bookOrderRepositoryMock.Verify(repo => repo.FindByIdAsync(bookOrderId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        VerifyErrorLog("Error occurred while updating status for book order");
    }
}
