using AutoMapper;

using Library.API.Domain.Models;
using Library.API.DTOs;

namespace Library.API.Mapping;

public class DtoToModelProfile : Profile
{
    public DtoToModelProfile()
    {
        CreateMap<SaveAuthorDto, Author>();
    }
}
