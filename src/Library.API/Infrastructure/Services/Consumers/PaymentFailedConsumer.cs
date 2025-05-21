using MassTransit;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.Events.Messages;

namespace Library.API.Infrastructure.Services.Consumers;

public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly IBookOrderService _service;

    public PaymentFailedConsumer(IBookOrderService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        await _service.UpdateStatusAsync(context.Message.OrderId, BookOrderStatus.PaymentFailed);
    }
}