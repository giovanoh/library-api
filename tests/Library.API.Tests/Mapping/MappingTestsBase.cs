using AutoMapper;

using Library.API.Mapping;

namespace Library.API.Tests.Mapping;

public class MappingTestsBase
{
    protected readonly IMapper Mapper;

    public MappingTestsBase()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DtoToModelProfile>();
            cfg.AddProfile<ModelToDtoProfile>();
            cfg.AddProfile<ModelToEventProfile>();
        });
        Mapper = configuration.CreateMapper();
    }

    [Fact]
    private void Should_Have_Valid_Configuration()
    {
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}