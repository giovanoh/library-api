using MassTransit;
using Microsoft.Extensions.Logging;

using Library.Events.Messages;

namespace Library.Checkout.Consumers;

public class OrderProcessingConsumer : IConsumer<OrderProcessingEvent>
{
    private readonly ILogger<OrderProcessingConsumer> _logger;

    public OrderProcessingConsumer(ILogger<OrderProcessingConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderProcessingEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Sending order: Id={OrderId}", evt.OrderId);
        var shippedEvent = new OrderShippedEvent
        {
            OrderId = evt.OrderId,
            ShippedAt = DateTime.UtcNow
        };
        await context.Publish(shippedEvent);
    }
}