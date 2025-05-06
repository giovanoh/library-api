using AutoMapper;

using Library.API.Domain.Models;
using Library.API.DTOs;

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
    }
}
