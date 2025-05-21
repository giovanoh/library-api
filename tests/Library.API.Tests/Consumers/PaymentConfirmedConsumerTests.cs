using MassTransit;
using Moq;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.API.Infrastructure.Services.Consumers;
using Library.Events.Messages;

namespace Library.API.Tests.Consumers;

public class PaymentConfirmedConsumerTests
{

    private readonly Mock<IBookOrderService> _bookOrderServiceMock;
    private readonly Mock<ConsumeContext<PaymentConfirmedEvent>> _contextMock;
    private readonly PaymentConfirmedConsumer _consumer;

    public PaymentConfirmedConsumerTests()
    {
        _bookOrderServiceMock = new Mock<IBookOrderService>();
        _contextMock = new Mock<ConsumeContext<PaymentConfirmedEvent>>();
        _consumer = new PaymentConfirmedConsumer(_bookOrderServiceMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldUpdateBookOrderStatus_WhenPaymentConfirmed()
    {
        // Arrange
        _contextMock.Setup(c => c.Message).Returns(new PaymentConfirmedEvent { OrderId = 123, ConfirmedAt = new DateTime(2025, 5, 1) });

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _bookOrderServiceMock.Verify(x => x.UpdateStatusAsync(123, BookOrderStatus.PaymentConfirmed), Times.Once);
    }

}
