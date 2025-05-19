using MassTransit;
using Microsoft.Extensions.Logging;

using Library.Events.Messages;

namespace Library.Checkout.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly ILogger<OrderPlacedConsumer> _logger;
    public OrderPlacedConsumer(ILogger<OrderPlacedConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Payment received: Id={OrderId}", evt.OrderId);
        // Simulate payment confirmation
        var paymentConfirmed = new PaymentConfirmedEvent
        {
            OrderId = evt.OrderId,
            ConfirmedAt = DateTime.UtcNow
        };
        await context.Publish(paymentConfirmed);
    }
}