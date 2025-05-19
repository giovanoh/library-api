using AutoMapper;

using Library.API.Domain.Models;
using Library.API.DTOs;
using Library.API.Extensions;

namespace Library.API.Mapping;

public class ModelToDtoProfile : Profile
{
    public ModelToDtoProfile()
    {
        CreateMap<Author, AuthorDto>();
        CreateMap<Book, BookDto>()
            .ForMember(
                dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author.Name)
            );
        CreateMap<BookOrder, BookOrderDto>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToDescription())
            );
        CreateMap<BookOrderItem, BookOrderItemDto>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom(src => src.Book.Title)
            );
    }
}
