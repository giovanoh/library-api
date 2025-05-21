using MassTransit;
using Moq;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.Infrastructure.Services.Consumers;
using Library.Events.Messages;

namespace Library.API.Tests.Consumers;

public class PaymentFailedConsumerTests
{

    private readonly Mock<IBookOrderService> _bookOrderServiceMock;
    private readonly Mock<ConsumeContext<PaymentFailedEvent>> _contextMock;
    private readonly PaymentFailedConsumer _consumer;

    public PaymentFailedConsumerTests()
    {
        _bookOrderServiceMock = new Mock<IBookOrderService>();
        _contextMock = new Mock<ConsumeContext<PaymentFailedEvent>>();
        _consumer = new PaymentFailedConsumer(_bookOrderServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldUpdateBookOrderStatus_WhenPaymentFailed()
    {
        // Arrange
        _contextMock.Setup(c => c.Message).Returns(new PaymentFailedEvent { OrderId = 123, Reason = "Insufficient funds", FailedAt = new DateTime(2025, 5, 1) });

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _bookOrderServiceMock.Verify(x => x.UpdateStatusAsync(123, BookOrderStatus.PaymentFailed), Times.Once);
    }

}
