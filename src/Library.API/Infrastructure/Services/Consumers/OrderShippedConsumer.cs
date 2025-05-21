using MassTransit;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.Events.Messages;

namespace Library.API.Infrastructure.Services.Consumers;

public class OrderShippedConsumer : IConsumer<OrderShippedEvent>
{
    private readonly IBookOrderService _service;

    public OrderShippedConsumer(IBookOrderService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<OrderShippedEvent> context)
    {
        await _service.UpdateStatusAsync(context.Message.OrderId, BookOrderStatus.Shipped);
    }
}