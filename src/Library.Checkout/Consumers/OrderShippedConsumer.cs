using MassTransit;
using Microsoft.Extensions.Logging;

using Library.Events.Messages;

namespace Library.Checkout.Consumers;

public class OrderShippedConsumer : IConsumer<OrderShippedEvent>
{
    private readonly ILogger<OrderShippedConsumer> _logger;

    public OrderShippedConsumer(ILogger<OrderShippedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderShippedEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Delivering order: Id={OrderId}", evt.OrderId);
        var deliveredEvent = new OrderDeliveredEvent
        {
            OrderId = evt.OrderId,
            DeliveredAt = DateTime.UtcNow
        };
        await context.Publish(deliveredEvent);
    }
}