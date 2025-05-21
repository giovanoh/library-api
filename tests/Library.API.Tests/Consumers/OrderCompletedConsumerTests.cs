using MassTransit;
using Moq;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.Infrastructure.Services.Consumers;
using Library.Events.Messages;

namespace Library.API.Tests.Consumers;

public class OrderCompletedConsumerTests
{

    private readonly Mock<IBookOrderService> _bookOrderServiceMock;
    private readonly Mock<ConsumeContext<OrderCompletedEvent>> _contextMock;
    private readonly OrderCompletedConsumer _consumer;

    public OrderCompletedConsumerTests()
    {
        _bookOrderServiceMock = new Mock<IBookOrderService>();
        _contextMock = new Mock<ConsumeContext<OrderCompletedEvent>>();
        _consumer = new OrderCompletedConsumer(_bookOrderServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldUpdateBookOrderStatus_WhenOrderCompleted()
    {
        // Arrange
        _contextMock.Setup(c => c.Message).Returns(new OrderCompletedEvent { OrderId = 123, CompletedAt = new DateTime(2025, 5, 1) });

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _bookOrderServiceMock.Verify(x => x.UpdateStatusAsync(123, BookOrderStatus.Completed), Times.Once);
    }

}
