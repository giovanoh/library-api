using MassTransit;
using Microsoft.Extensions.Logging;

using Library.Events.Messages;

namespace Library.Checkout.Consumers;

public class OrderDeliveredConsumer : IConsumer<OrderDeliveredEvent>
{
    private readonly ILogger<OrderDeliveredConsumer> _logger;

    public OrderDeliveredConsumer(ILogger<OrderDeliveredConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderDeliveredEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Finalizing order: Id={OrderId}", evt.OrderId);
        var completedEvent = new OrderCompletedEvent
        {
            OrderId = evt.OrderId,
            CompletedAt = DateTime.UtcNow
        };
        await context.Publish(completedEvent);
    }
}