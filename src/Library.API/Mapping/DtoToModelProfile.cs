using AutoMapper;

using Library.API.Domain.Models;
using Library.API.DTOs;

namespace Library.API.Mapping;

public class DtoToModelProfile : Profile
{
    public DtoToModelProfile()
    {
        CreateMap<SaveAuthorDto, Author>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Books, opt => opt.Ignore());
        CreateMap<SaveBookDto, Book>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore());
    }
}
