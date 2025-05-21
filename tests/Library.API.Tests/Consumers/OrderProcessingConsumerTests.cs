using MassTransit;
using Moq;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.Infrastructure.Services.Consumers;
using Library.Events.Messages;

namespace Library.API.Tests.Consumers;

public class OrderProcessingConsumerTests
{

    private readonly Mock<IBookOrderService> _bookOrderServiceMock;
    private readonly Mock<ConsumeContext<OrderProcessingEvent>> _contextMock;
    private readonly OrderProcessingConsumer _consumer;

    public OrderProcessingConsumerTests()
    {
        _bookOrderServiceMock = new Mock<IBookOrderService>();
        _contextMock = new Mock<ConsumeContext<OrderProcessingEvent>>();
        _consumer = new OrderProcessingConsumer(_bookOrderServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldUpdateBookOrderStatus_WhenOrderProcessing()
    {
        // Arrange
        _contextMock.Setup(c => c.Message).Returns(new OrderProcessingEvent { OrderId = 123, ProcessingAt = new DateTime(2025, 5, 1) });

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _bookOrderServiceMock.Verify(x => x.UpdateStatusAsync(123, BookOrderStatus.Processing), Times.Once);
    }

}
