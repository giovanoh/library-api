using MassTransit;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.Events.Messages;

namespace Library.API.Infrastructure.Services.Consumers;

public class OrderCompletedConsumer : IConsumer<OrderCompletedEvent>
{
    private readonly IBookOrderService _service;

    public OrderCompletedConsumer(IBookOrderService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        await _service.UpdateStatusAsync(context.Message.OrderId, BookOrderStatus.Completed);
    }
}