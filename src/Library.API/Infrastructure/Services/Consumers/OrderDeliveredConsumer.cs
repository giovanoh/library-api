using MassTransit;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.Events.Messages;

namespace Library.API.Infrastructure.Services.Consumers;

public class OrderDeliveredConsumer : IConsumer<OrderDeliveredEvent>
{
    private readonly IBookOrderService _service;

    public OrderDeliveredConsumer(IBookOrderService service)
    {
        _service = service;
    }
    public async Task Consume(ConsumeContext<OrderDeliveredEvent> context)
    {
        await _service.UpdateStatusAsync(context.Message.OrderId, BookOrderStatus.Delivered);
    }
}