using MassTransit;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.Events.Messages;

namespace Library.API.Infrastructure.Services.Consumers;

public class PaymentConfirmedConsumer : IConsumer<PaymentConfirmedEvent>
{
    private readonly IBookOrderService _service;

    public PaymentConfirmedConsumer(IBookOrderService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<PaymentConfirmedEvent> context)
    {
        await _service.UpdateStatusAsync(context.Message.OrderId, BookOrderStatus.PaymentConfirmed);
    }
}