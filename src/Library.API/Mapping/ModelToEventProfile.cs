using AutoMapper;

using Library.API.Domain.Models;
using Library.API.Extensions;
using Library.Events.Messages;

public class ModelToEventProfile : Profile
{
    public ModelToEventProfile()
    {
        CreateMap<BookOrder, OrderPlacedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToDescription()));

        CreateMap<BookOrderItem, OrderPlacedEventItem>();
    }
}