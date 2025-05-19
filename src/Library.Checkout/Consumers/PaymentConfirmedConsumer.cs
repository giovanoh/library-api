using MassTransit;
using Microsoft.Extensions.Logging;

using Library.Events.Messages;

namespace Library.Checkout.Consumers;

public class PaymentConfirmedConsumer : IConsumer<PaymentConfirmedEvent>
{
    private readonly ILogger<PaymentConfirmedConsumer> _logger;

    public PaymentConfirmedConsumer(ILogger<PaymentConfirmedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentConfirmedEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Processing order: Id={OrderId}", evt.OrderId);
        var processingEvent = new OrderProcessingEvent
        {
            OrderId = evt.OrderId,
            ProcessingAt = DateTime.UtcNow
        };
        await context.Publish(processingEvent);
    }
}