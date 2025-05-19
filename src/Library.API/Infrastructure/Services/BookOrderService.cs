using System.Diagnostics;

using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;

using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Domain.Services;
using Library.API.Domain.Services.Communication;
using ServiceResponse = Library.API.Domain.Services.Communication.Response<Library.API.Domain.Models.BookOrder>;
using Library.Events.Messages;

namespace Library.API.Infrastructure.Services;

public class BookOrderService : BaseService, IBookOrderService
{
    private readonly IBookOrderRepository _bookOrderRepository;
    private readonly ActivitySource _activitySource;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public BookOrderService(
        IBookOrderRepository bookOrderRepository,
        IUnitOfWork unitOfWork,
        ILogger<BookOrderService> logger,
        ActivitySource activitySource,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
        : base(unitOfWork, logger)
    {
        _bookOrderRepository = bookOrderRepository;
        _activitySource = activitySource;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ServiceResponse> FindByIdAsync(int id)
    {
        using var activity = _activitySource.StartActivity("Service: BookOrderService.FindByIdAsync");
        try
        {
            var bookOrder = await _bookOrderRepository.FindByIdAsync(id);
            if (bookOrder == null)
                return ServiceResponse.NotFound($"Book order with id {id} was not found");
            return ServiceResponse.Ok(bookOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while finding book order {BookOrderId}.", id);
            return ServiceResponse.Fail(
                "An error occurred while retrieving the book order",
                ErrorType.DatabaseError);
        }
    }

    public async Task<ServiceResponse> AddAsync(BookOrder bookOrder)
    {
        using var activity = _activitySource.StartActivity("Service: BookOrderService.AddAsync");
        try
        {
            if (bookOrder.Items.Count == 0)
                return ServiceResponse.Fail("Book order must have at least one item", ErrorType.ValidationError);

            await _bookOrderRepository.AddAsync(bookOrder);
            await _unitOfWork.CompleteAsync();

            var orderWithRelations = await _bookOrderRepository.FindByIdAsync(bookOrder.Id);

            var orderPlacedEvent = _mapper.Map<OrderPlacedEvent>(orderWithRelations);
            await _publishEndpoint.Publish(orderPlacedEvent);

            return ServiceResponse.Ok(orderWithRelations!);
        }
        catch (MessageException ex)
        {
            _logger.LogError(ex, "Error occurred while publishing order placed event for book order {BookOrderId}.", bookOrder.Id);
            return ServiceResponse.Fail(
                "An error occurred while publishing the order event",
                ErrorType.MessageBrokerError);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error occurred while creating book order {BookOrderId}.", bookOrder.Id);
            return ServiceResponse.Fail(
                "An error occurred while saving the book order",
                ErrorType.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating book order {BookOrderId}.", bookOrder.Id);
            return ServiceResponse.Fail(
                "An unexpected error occurred while processing your request",
                ErrorType.Unknown);
        }
    }

    public async Task<ServiceResponse> UpdateStatusAsync(int orderId, BookOrderStatus status)
    {
        using var activity = _activitySource.StartActivity("Service: BookOrderService.UpdateStatusAsync");
        try
        {
            var bookOrder = await _bookOrderRepository.FindByIdAsync(orderId);
            if (bookOrder == null)
                return ServiceResponse.NotFound($"Book order with id {orderId} was not found");

            bookOrder.Status = status;
            await _unitOfWork.CompleteAsync();

            return ServiceResponse.Ok(bookOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating status for book order {BookOrderId}.", orderId);
            return ServiceResponse.Fail(
                "An error occurred while updating the book order status",
                ErrorType.DatabaseError);
        }
    }
}
