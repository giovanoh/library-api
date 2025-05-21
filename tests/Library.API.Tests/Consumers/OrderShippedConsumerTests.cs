using MassTransit;
using Moq;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.Infrastructure.Services.Consumers;
using Library.Events.Messages;

namespace Library.API.Tests.Consumers;

public class OrderShippedConsumerTests
{

    private readonly Mock<IBookOrderService> _bookOrderServiceMock;
    private readonly Mock<ConsumeContext<OrderShippedEvent>> _contextMock;
    private readonly OrderShippedConsumer _consumer;

    public OrderShippedConsumerTests()
    {
        _bookOrderServiceMock = new Mock<IBookOrderService>();
        _contextMock = new Mock<ConsumeContext<OrderShippedEvent>>();
        _consumer = new OrderShippedConsumer(_bookOrderServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldUpdateBookOrderStatus_WhenOrderShipped()
    {
        // Arrange
        _contextMock.Setup(c => c.Message).Returns(new OrderShippedEvent { OrderId = 123, ShippedAt = new DateTime(2025, 5, 1) });

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _bookOrderServiceMock.Verify(x => x.UpdateStatusAsync(123, BookOrderStatus.Shipped), Times.Once);
    }

}
