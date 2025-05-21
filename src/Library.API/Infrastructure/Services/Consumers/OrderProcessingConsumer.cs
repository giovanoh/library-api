using MassTransit;

using Library.API.Domain.Models;
using Library.API.Domain.Services;
using Library.Events.Messages;

namespace Library.API.Infrastructure.Services.Consumers;

public class OrderProcessingConsumer : IConsumer<OrderProcessingEvent>
{
    private readonly IBookOrderService _service;

    public OrderProcessingConsumer(IBookOrderService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<OrderProcessingEvent> context)
    {
        await _service.UpdateStatusAsync(context.Message.OrderId, BookOrderStatus.Processing);
    }
}