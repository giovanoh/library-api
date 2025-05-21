using MassTransit;
using Moq;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.Infrastructure.Services.Consumers;
using Library.Events.Messages;

namespace Library.API.Tests.Consumers;

public class OrderDeliveredConsumerTests
{

    private readonly Mock<IBookOrderService> _bookOrderServiceMock;
    private readonly Mock<ConsumeContext<OrderDeliveredEvent>> _contextMock;
    private readonly OrderDeliveredConsumer _consumer;

    public OrderDeliveredConsumerTests()
    {
        _bookOrderServiceMock = new Mock<IBookOrderService>();
        _contextMock = new Mock<ConsumeContext<OrderDeliveredEvent>>();
        _consumer = new OrderDeliveredConsumer(_bookOrderServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldUpdateBookOrderStatus_WhenOrderDelivered()
    {
        // Arrange
        _contextMock.Setup(c => c.Message).Returns(new OrderDeliveredEvent { OrderId = 123, DeliveredAt = new DateTime(2025, 5, 1) });

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _bookOrderServiceMock.Verify(x => x.UpdateStatusAsync(123, BookOrderStatus.Delivered), Times.Once);
    }

}
